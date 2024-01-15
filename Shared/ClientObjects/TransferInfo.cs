using Shared.DBObjects.TransferData;

namespace SharedClass.ClientObjects
{
	public class TransferInfo
	{

		//Who?
		public string AccountNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		//Is this person is paying to you?
		public bool IsSender { get; set; }
		public string Title { get; set; }
		public double Price { get; set; }
		public DateTime TimeStamp { get; set; }

		public TransferInfo() { }

		public TransferInfo(Transfer data, string accountNum, string firstName, string lastName, bool sender)
		{
			AccountNumber = accountNum;
			FirstName = firstName;
			LastName = lastName;
			IsSender = sender;
			Title = data.Title;
			Price = data.Price;
			TimeStamp = data.TimeStamp;
		}
	}
}
