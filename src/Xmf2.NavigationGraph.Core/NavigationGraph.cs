using System;
using System.Collections.Generic;
using System.Linq;
using Xmf2.NavigationGraph.Core.Extensions;
using Xmf2.NavigationGraph.Core.Graph;

namespace Xmf2.NavigationGraph.Core
{
	internal class NavigationGraph
	{
		private readonly Dictionary<Guid, Node> _nodePerScreenIds = new Dictionary<Guid, Node>();
		private readonly List<Node> _entryPoints = new List<Node>();

		public void Add(ScreenDefinition from, ScreenDefinition to)
		{
			if (to is null)
			{
				throw new ArgumentNullException(nameof(to), "Null value for destination is not allowed");
			}

			if (!_nodePerScreenIds.TryGetValue(to.Id, out Node node))
			{
				node = new Node(to, isEntryPoint: from is null);
				_nodePerScreenIds.Add(node.Screen.Id, node);
			}

			if (from is null)
			{
				_entryPoints.Add(node);
				return;
			}

			if (!_nodePerScreenIds.TryGetValue(from.Id, out Node fromNode))
			{
				throw new InvalidOperationException("Source has not been added to graph before destination, this is not allowed");
			}

			fromNode.NextNodes.Add(node);
		}

		public IList<ScreenInstance> FindWithRoute(string route)
		{
			if (string.IsNullOrWhiteSpace(route))
			{
				throw new InvalidOperationException("Route must not be empty or null");
			}

			//TODO: can set up a cache with found route in order to help the system
			string[] routeParts = route.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
			(List<ScreenInstance> resultNodes, _, bool resultAmbiguity) = FindBest(_entryPoints, routeParts, 0);

			if (resultAmbiguity)
			{
				throw new InvalidOperationException($"Ambiguous route {route}, specify parameter name if needed");
			}

			return resultNodes;

			(List<ScreenInstance> nodes, int score, bool ambiguity) FindBest(List<Node> nodes, string[] parts, int index)
			{
				(List<ScreenInstance> nodes, int score) result = (null, 0);
				bool ambiguity = false;
				for (int i = 0; i < nodes.Count; i++)
				{
					(List<ScreenInstance> nodes, int score, bool ambiguity) proposal = FindForRoute(nodes[i], parts, index);
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

			(List<ScreenInstance> nodes, int score, bool ambiguity) FindForRoute(Node currentNode, string[] parts, int index)
			{
				(int currentNodeScore, string routeParameter) = RoutePartScore(currentNode, parts[index]);
				if (currentNodeScore == 0)
				{
					return (null, 0, false);
				}

				ScreenInstance current = new ScreenInstance(currentNode.Screen, routeParameter, null);

				if (index + 1 == parts.Length)
				{
					return (new List<ScreenInstance>
					{
						current
					}, currentNodeScore, false);
				}

				(List<ScreenInstance> nodes, int score, bool ambiguity) result = FindBest(currentNode.NextNodes, parts, index + 1);
				result.nodes.Insert(0, current);

				return result;
			}

			(int score, string parameter) RoutePartScore(Node node, string part)
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

		public IList<ScreenInstance> FindBestStack(List<ScreenInstance> currentStack, ScreenInstance screen)
		{
			if (!_nodePerScreenIds.TryGetValue(screen.Definition.Id, out Node destinationNode))
			{
				throw new InvalidOperationException("This screen has not been registered");
			}

			ScreenNode destinationScreen = new ScreenNode(destinationNode, screen);
			List<ScreenNode> currentScreenStack = currentStack.ConvertAll(x => new ScreenNode(_nodePerScreenIds[x.Definition.Id], x));

			// If nothing is on navigation stack,
			//	we need to find a path in the tree to get to the screen, a simple BFS will works, just need to take the multi entry points into account
			// Else
			//	We need to find the shortest path to go from the current screen to the other
			//	correct way links must have priority (but if a down links is taken, only down links can taken after that)
			//	can only take up links that are in the navigation stack

			return FindPathToScreenWithBFS(currentScreenStack, destinationScreen).ConvertAll(x => x.ScreenInstance);
		}

		private List<ScreenNode> FindPathToScreenWithBFS(List<ScreenNode> navigationStack, ScreenNode destination)
		{
			Dictionary<Node, ScreenNode> links = new Dictionary<Node, ScreenNode>(_nodePerScreenIds.Count);

			Queue<(int level, ScreenNode node)> toProcess = new Queue<(int level, ScreenNode node)>(16);
			HashSet<Node> usedEntryPoints = new HashSet<Node>();

			// try to find with backtracking the less possible in the stack
			for (int navigationStackIndex = navigationStack.Count - 1; navigationStackIndex >= 0; navigationStackIndex--)
			{
				ScreenNode displayedScreen = navigationStack[navigationStackIndex];

				if (displayedScreen.Node == destination.Node && displayedScreen.ScreenInstance == destination.ScreenInstance)
				{
					return navigationStack.Sublist(0, navigationStackIndex + 1);
				}

				toProcess.Enqueue((1, displayedScreen));

				if (displayedScreen.Node.IsEntryPoint)
				{
					usedEntryPoints.Add(displayedScreen.Node);
				}

				if (FindPath(destination, toProcess, links, out IList<ScreenNode> resultFromStack))
				{
					List<ScreenNode> result = navigationStack.Sublist(0, navigationStackIndex);
					result.AddRange(resultFromStack);
					return result;
				}
			}

			// if not possible, we start from each entry point
			foreach (Node entryPoint in _entryPoints)
			{
				if (entryPoint == destination.Node)
				{
					return new List<ScreenNode>
					{
						destination
					};
				}

				if (usedEntryPoints.Contains(entryPoint))
				{
					continue;
				}

				toProcess.Enqueue((1, new ScreenNode(entryPoint, new ScreenInstance(entryPoint.Screen, parameter: null, viewModelCreator: null))));
			}

			if (FindPath(destination, toProcess, links, out IList<ScreenNode> resultFromEntryPoints))
			{
				return resultFromEntryPoints.ToList();
			}

			throw new InvalidOperationException("No way to get to this screen");

			bool FindPath(ScreenNode internalDestination, Queue<(int level, ScreenNode node)> internalToProcess, Dictionary<Node, ScreenNode> internalLinks, out IList<ScreenNode> nodes)
			{
				while (internalToProcess.Count > 0)
				{
					(int level, ScreenNode n) = internalToProcess.Dequeue();

					foreach (Node childNode in n.Node.NextNodes)
					{
						if (internalLinks.ContainsKey(childNode))
						{
							continue;
						}

						if (childNode == internalDestination.Node)
						{
							// backtrack to get the path and return
							ScreenNode[] result = new ScreenNode[level + 1]; //+1 for current child

							result[level] = internalDestination;
							result[level - 1] = n;
							for (int i = level - 2; i >= 0; --i)
							{
								n = internalLinks[n.Node];
								result[i] = n;
							}

							nodes = result;
							return true;
						}

						internalLinks.Add(childNode, n); //links for backtracking
						internalToProcess.Enqueue((level + 1, new ScreenNode(childNode, new ScreenInstance(childNode.Screen, parameter: null, viewModelCreator: null))));
					}
				}

				nodes = null;
				return false;
			}
		}
	}
}