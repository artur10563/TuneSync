namespace Domain.Entities.Shared
{
	public abstract class EntityBase
	{
		public Guid Guid { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime ModifiedAt { get; set; }

		public EntityBase()
		{
			Guid = Guid.NewGuid();
			CreatedAt = DateTime.Now.ToUniversalTime();
			ModifiedAt = DateTime.Now.ToUniversalTime();
		}
	}
}
