using Domain.Entities.Shared;

namespace Domain.Entities
{
	public class Song : EntityBase
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string AudioPath { get; set; }
	}
}
