using Google.Cloud.Firestore; //TODO: Clean arch violation

namespace TuneSync.Domain.Entities
{
	[FirestoreData]
	public class Song
	{
		[FirestoreProperty]
		public int Id { get; set; }

		[FirestoreProperty]
		public string Name { get; set; }

		[FirestoreProperty]
		public string VideoUrl { get; set; }
		
		[FirestoreProperty]
		public string AudioUrl { get; set; }
	}
}
