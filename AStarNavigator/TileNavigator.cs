using System.Collections.Generic;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;

namespace AStarNavigator
{
	public class TileNavigator : ITileNavigator
	{
		private readonly IBlockedProvider blockedProvider;
		private readonly INeighborProvider neighborProvider;

		private readonly IDistanceAlgorithm distanceAlgorithm;
		private readonly IDistanceAlgorithm heuristicAlgorithm;

		public TileNavigator(
			IBlockedProvider blockedProvider,
			INeighborProvider neighborProvider,
			IDistanceAlgorithm distanceAlgorithm,
			IDistanceAlgorithm heuristicAlgorithm)
		{
			this.blockedProvider = blockedProvider;
			this.neighborProvider = neighborProvider;

			this.distanceAlgorithm = distanceAlgorithm;
			this.heuristicAlgorithm = heuristicAlgorithm;
		}

		public IEnumerable<Tile> Navigate(Tile from, Tile to, int maxAttempts = int.MaxValue)
		{
			var closed = new HashSet<Tile>();
			var open = new HashSet<Tile>() {from};

			var path = new Dictionary<Tile, Tile>();

			from.FScore = heuristicAlgorithm.Calculate(from, to);

			int noOfAttempts = 0;

			Tile highScore = from;
			while (open.Count != 0)
			{
				var current = highScore;
				if (current.Equals(to) || ++noOfAttempts > maxAttempts)
				{
					return ReconstructPath(path, current);
				}

				open.Remove(current);
				closed.Add(current);

				foreach (Tile neighbor in neighborProvider.GetNeighbors(current))
				{
					if (closed.Contains(neighbor) || blockedProvider.IsBlocked(neighbor))
					{
						continue;
					}

					var tentativeG = current.GScore + distanceAlgorithm.Calculate(current, neighbor);

					if (!open.Add(neighbor) && tentativeG >= neighbor.GScore)
					{
						continue;
					}

					path[neighbor] = current;

					neighbor.GScore = tentativeG;
					//fScore[neighbor] = gScore[neighbor] + heuristicAlgorithm.Calculate(neighbor, to);
					neighbor.FScore = neighbor.GScore + heuristicAlgorithm.Calculate(neighbor, to);
					if (neighbor.FScore <= highScore.FScore) highScore = neighbor;
				}
			}

			return null;
		}

		private IEnumerable<Tile> ReconstructPath(
			IDictionary<Tile, Tile> path,
			Tile current)
		{
			List<Tile> totalPath = new List<Tile>() {current};

			while (path.ContainsKey(current))
			{
				current = path[current];
				totalPath.Insert(0, current);
			}

			totalPath.RemoveAt(0);

			return totalPath;
		}
	}
}