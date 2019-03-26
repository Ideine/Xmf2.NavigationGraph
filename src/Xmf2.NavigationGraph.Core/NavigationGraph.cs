using System;
using System.Collections.Generic;
using System.Linq;
using Xmf2.NavigationGraph.Core.Extensions;
using Xmf2.NavigationGraph.Core.Graph;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	internal class NavigationGraph<TViewModel> where TViewModel : IViewModel
	{
		private readonly Dictionary<Guid, Node<TViewModel>> _nodePerScreenIds = new Dictionary<Guid, Node<TViewModel>>();
		private readonly List<Node<TViewModel>> _entryPoints = new List<Node<TViewModel>>();

		public void Add(ScreenDefinition<TViewModel> from, ScreenDefinition<TViewModel> to)
		{
			if (to is null)
			{
				throw new ArgumentNullException(nameof(to), "Null value for destination is not allowed");
			}

			if (!_nodePerScreenIds.TryGetValue(to.Id, out var node))
			{
				node = new Node<TViewModel>(to, isEntryPoint: from is null);
				_nodePerScreenIds.Add(node.Screen.Id, node);
			}

			if (from is null)
			{
				_entryPoints.Add(node);
				return;
			}

			if (!_nodePerScreenIds.TryGetValue(from.Id, out Node<TViewModel> fromNode))
			{
				throw new InvalidOperationException("Source has not been added to graph before destination, this is not allowed");
			}

			fromNode.NextNodes.Add(node);
		}

		public IList<ScreenInstance<TViewModel>> FindWithRoute(string route)
		{
			if (string.IsNullOrWhiteSpace(route))
			{
				throw new InvalidOperationException("Route must not be empty or null");
			}

			//TODO: can set up a cache with found route in order to help the system
			string[] routeParts = route.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
			(List<ScreenInstance<TViewModel>> resultNodes, _, bool resultAmbiguity) = FindBest(_entryPoints, routeParts, 0);

			if (resultAmbiguity)
			{
				throw new InvalidOperationException($"Ambiguous route {route}, specify parameter name if needed");
			}

			return resultNodes;

			(List<ScreenInstance<TViewModel>> nodes, int score, bool ambiguity) FindBest(List<Node<TViewModel>> nodes, string[] parts, int index)
			{
				(List<ScreenInstance<TViewModel>> nodes, int score) result = (null, 0);
				bool ambiguity = false;
				for (int i = 0 ; i < nodes.Count ; i++)
				{
					(List<ScreenInstance<TViewModel>> nodes, int score, bool ambiguity) proposal = FindForRoute(nodes[i], parts, index);
					if (proposal.nodes is null || result.score > proposal.score)
					{
						continue;
					}

					if (result.score == proposal.score && result.score > 0)
					{
						ambiguity = true;
						continue;
					}

					ambiguity = proposal.ambiguity;
					result = (proposal.nodes, proposal.score);
				}

				return (result.nodes, result.score, ambiguity);
			}

			(List<ScreenInstance<TViewModel>> nodes, int score, bool ambiguity) FindForRoute(Node<TViewModel> currentNode, string[] parts, int index)
			{
				(int currentNodeScore, string routeParameter) = RoutePartScore(currentNode, parts[index]);
				if (currentNodeScore == 0)
				{
					return (null, 0, false);
				}

				var current = new ScreenInstance<TViewModel>(currentNode.Screen, routeParameter, null);

				if (index + 1 == parts.Length)
				{
					return (new List<ScreenInstance<TViewModel>>
					{
						current
					}, currentNodeScore, false);
				}

				(List<ScreenInstance<TViewModel>> nodes, int score, bool ambiguity) result = FindBest(currentNode.NextNodes, parts, index + 1);
				result.nodes.Insert(0, current);

				return result;
			}

			(int score, string parameter) RoutePartScore(Node<TViewModel> node, string part)
			{
				if (node.Screen.IsParameterRoute)
				{
					if (part.StartsWith($"{node.Screen.ParameterName}:"))
					{
						// if parameter name match, the score is higher than general scoring
						return (3, part.Substring(node.Screen.ParameterName.Length + 1));
					}

					// this is a parameterized route so it can match but with a lower score
					return (1, part);
				}

				if (part == node.Screen.RelativeRoute)
				{
					return (4, null);
				}

				return (0, null);
			}
		}

		public IList<ScreenInstance<TViewModel>> FindBestStack(List<ScreenInstance<TViewModel>> currentStack, ScreenInstance<TViewModel> screen)
		{
			if (!_nodePerScreenIds.TryGetValue(screen.Definition.Id, out var destinationNode))
			{
				throw new InvalidOperationException("This screen has not been registered");
			}

			var destinationScreen = new ScreenNode<TViewModel>(destinationNode, screen);
			var currentScreenStack = currentStack.ConvertAll(x => new ScreenNode<TViewModel>(_nodePerScreenIds[x.Definition.Id], x));

			// If nothing is on navigation stack,
			//	we need to find a path in the tree to get to the screen, a simple BFS will works, just need to take the multi entry points into account
			// Else
			//	We need to find the shortest path to go from the current screen to the other
			//	correct way links must have priority (but if a down links is taken, only down links can taken after that)
			//	can only take up links that are in the navigation stack

			return FindPathToScreenWithBFS(currentScreenStack, destinationScreen).ConvertAll(x => x.ScreenInstance);
		}

		private List<ScreenNode<TViewModel>> FindPathToScreenWithBFS(List<ScreenNode<TViewModel>> navigationStack, ScreenNode<TViewModel> destination)
		{
			Dictionary<Node<TViewModel>, ScreenNode<TViewModel>> links = new Dictionary<Node<TViewModel>, ScreenNode<TViewModel>>(_nodePerScreenIds.Count);

			Queue<(int level, ScreenNode<TViewModel> node)> toProcess = new Queue<(int level, ScreenNode<TViewModel> node)>(16);
			HashSet<Node<TViewModel>> usedEntryPoints = new HashSet<Node<TViewModel>>();

			// try to find with backtracking the less possible in the stack
			for (int navigationStackIndex = navigationStack.Count - 1 ; navigationStackIndex >= 0 ; navigationStackIndex--)
			{
				var displayedScreen = navigationStack[navigationStackIndex];

				if (displayedScreen.Node == destination.Node && displayedScreen.ScreenInstance == destination.ScreenInstance)
				{
					return navigationStack.Sublist(0, navigationStackIndex + 1);
				}

				toProcess.Enqueue((1, displayedScreen));

				if (displayedScreen.Node.IsEntryPoint)
				{
					usedEntryPoints.Add(displayedScreen.Node);
				}

				if (FindPath(destination, toProcess, links, out IList<ScreenNode<TViewModel>> resultFromStack))
				{
					List<ScreenNode<TViewModel>> result = navigationStack.Sublist(0, navigationStackIndex);
					result.AddRange(resultFromStack);
					return result;
				}
			}

			// if not possible, we start from each entry point
			foreach (var entryPoint in _entryPoints)
			{
				if (entryPoint == destination.Node)
				{
					return new List<ScreenNode<TViewModel>>
					{
						destination
					};
				}

				if (usedEntryPoints.Contains(entryPoint))
				{
					continue;
				}

				toProcess.Enqueue((1, new ScreenNode<TViewModel>(entryPoint, new ScreenInstance<TViewModel>(entryPoint.Screen, parameter: null, viewModelCreator: null))));
			}

			if (FindPath(destination, toProcess, links, out IList<ScreenNode<TViewModel>> resultFromEntryPoints))
			{
				return resultFromEntryPoints.ToList();
			}

			throw new InvalidOperationException("No way to get to this screen");

			bool FindPath(ScreenNode<TViewModel> internalDestination, Queue<(int level, ScreenNode<TViewModel> node)> internalToProcess, Dictionary<Node<TViewModel>, ScreenNode<TViewModel>> internalLinks, out IList<ScreenNode<TViewModel>> nodes)
			{
				while (internalToProcess.Count > 0)
				{
					(int level, ScreenNode<TViewModel> n) = internalToProcess.Dequeue();

					foreach (var childNode in n.Node.NextNodes)
					{
						if (internalLinks.ContainsKey(childNode))
						{
							continue;
						}

						if (childNode == internalDestination.Node)
						{
							// backtrack to get the path and return
							var result = new ScreenNode<TViewModel>[level + 1]; //+1 for current child

							result[level] = internalDestination;
							result[level - 1] = n;
							for (int i = level - 2 ; i >= 0 ; --i)
							{
								n = internalLinks[n.Node];
								result[i] = n;
							}

							nodes = result;
							return true;
						}

						internalLinks.Add(childNode, n); //links for backtracking
						internalToProcess.Enqueue((level + 1, new ScreenNode<TViewModel>(childNode, new ScreenInstance<TViewModel>(childNode.Screen, parameter: null, viewModelCreator: null))));
					}
				}

				nodes = null;
				return false;
			}
		}
	}
}