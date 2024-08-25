using Google.Cloud.Firestore;
using TuneSync.Domain.Entities.Shared; //TODO: Clean arch violation

namespace TuneSync.Domain.Entities
{
	[FirestoreData]
	public class Song : EntityBase
	{

		[FirestoreProperty]
		public string Name { get; set; }

		[FirestoreProperty]
		public string VideoUrl { get; set; }
		
		[FirestoreProperty]
		public string AudioUrl { get; set; }
	}
}
