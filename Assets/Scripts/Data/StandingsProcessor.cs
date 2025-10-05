using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
	public class StandingsProcessor
	{
		private IEnumerable<IDriverData> latestDriverData;
		
		public void UpdateData(IEnumerable<IDriverData> data)
		{
			latestDriverData = data;
		}

		public IEnumerable<IDriverData> GetSortedResultsForClass(string className)
		{
			var driversForClass = latestDriverData.Where(d => d.DriverClass == className);
			return driversForClass.OrderByDescending(d => d.DriverPoints);
		}
	}
}