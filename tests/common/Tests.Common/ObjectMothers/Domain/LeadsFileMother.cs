using Domain.Prospecting.Entities;

namespace Tests.Common.ObjectMothers.Domain;

public class LeadsFileMother
{
	private LeadsFileMother() { }

	public static LeadsFile File1()
		=> LeadsFile.Create(
			TimeProvider.System.GetLocalNow(),
			userId: "6BF47D48-54BB-4DC8-B0E9-43AB7B2F548B",
			fileId: "B65BBF7E-4ACA-40F9-9AB1-7379BD2C8EE9",
			fileName: "file1.csv",
			pathOrContainerName: "leads-files",
			description: default!);

	public static LeadsFile File2()
		=> LeadsFile.Create(
			TimeProvider.System.GetLocalNow(),
			userId: "5EE2E976-0C26-4E95-9EB8-07368D44F2B2",
			fileId: "FE0E5345-AF5B-4AE4-A230-719E866F4546",
			fileName: "file2.csv",
			pathOrContainerName: "leads-files",
			description: default!);

	public static IEnumerable<LeadsFile> LeadsFiles()
	{
		yield return File1();
		yield return File2();
	}
}