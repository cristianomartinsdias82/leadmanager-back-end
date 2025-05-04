using CrossCutting.FileStorage;
using CrossCutting.FileStorage.Azure;
using Domain.Prospecting.Entities;
using System.Net.Mime;
using System.Text;

namespace Tests.Common.ObjectMothers.CrossCutting.FileStorage;

public static class FileStorageMother
{
	public static IFile FromLeadsFile(LeadsFile leadsFile)
	{
		var fileBytes = Encoding.UTF8.GetBytes("test1 - file content...");

		return new AzureBlobFile
		{
			ContentLength = fileBytes.Length,
			FileBytes = fileBytes,
			ContentType = MediaTypeNames.Text.Plain,
			Location = leadsFile.PathOrContainerName,
			Name = leadsFile.FileName
		};
	}
}
