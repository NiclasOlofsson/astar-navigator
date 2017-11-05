using System;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Tile> Navigate(Tile from, Tile to)
        {
            var closed = new HashSet<Tile>();
            var open = new HashSet<Tile>() { from };

            var path = new Dictionary<Tile, Tile>();

            //var gScore = new Dictionary<Tile, double>();
            //gScore[from] = 0;

            //var fScore = new Dictionary<Tile, double>();
	        from.FScore = heuristicAlgorithm.Calculate(from, to);

	        Tile highScore = from;
	        //while (open.Any())
			while (open.Count != 0)
			{
				var current = highScore;
				if(!open.Contains(current))
				{
					current = open
						.OrderBy(c => c.FScore)
						.First();
				}

                if (current.Equals(to))
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
	                if (neighbor.FScore < highScore.FScore) highScore = neighbor;
                }
            }

            return null;
        }

        private IEnumerable<Tile> ReconstructPath(
            IDictionary<Tile, Tile> path,
            Tile current)
        {
            List<Tile> totalPath = new List<Tile>() { current };

            while (path.ContainsKey(current))
            {
                current = path[current];
                totalPath.Insert(0, current);
            }

            //totalPath.Reverse();
            totalPath.RemoveAt(0);

            return totalPath;
        }
    }
}
