﻿using System.IO;
using Hayden.WebServer.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hayden.WebServer.Controllers
{
	[ApiModeActionFilter(false)]
	[Route("")]
	public class ArchiveViewController : Controller
	{
		protected IOptions<ServerConfig> Config { get; set; }

		public ArchiveViewController(IOptions<ServerConfig> config)
		{
			Config = config;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View("~/View/Svelte.cshtml");
		}

		[HttpGet]
		[Route("{board}/thread/{threadid}")]
		public IActionResult ThreadIndex(string board, ulong threadid)
		{
			return View("~/View/Svelte.cshtml");
		}

		[HttpGet]
		[Route("board/{board}")]
		public IActionResult BoardIndex(string board)
		{
			return NotFound();
		}

		[HttpGet]
		[Route("board/{board}/page/{pageNumber}")]
		public IActionResult BoardIndexPaged(string board)
		{
			return NotFound();
		}

		[HttpGet]
		[Route("search")]
		public IActionResult Search()
		{
			return NotFound();
		}

		[HttpGet]
		[Route("Login")]
		public IActionResult Login()
		{
			return NotFound();
		}

		[HttpGet]
		[Route("Register")]
		public IActionResult Register()
		{
			return NotFound();
		}

		[NonAction]
		private static YotsubaThread ReadJson(string filename)
		{
			using StreamReader streamReader = new StreamReader(System.IO.File.OpenRead(filename));
			using JsonReader reader = new JsonTextReader(streamReader);

			return JToken.Load(reader).ToObject<YotsubaThread>();
		}

		///// <summary>
		///// This is a development method for importing JSON objects into the database.
		///// This will probably be reworked into an admin panel-only workflow
		///// </summary>
		//[Route("reprocess")]
		//[HttpGet]
		//public async Task<IActionResult> Reprocess([FromServices] HaydenDbContext dbContext)
		//{
		//	//foreach (var subfolder in Directory.EnumerateDirectories("G:\\utg-archive", "*", SearchOption.TopDirectoryOnly))
		//	//{
		//	//	string board = Path.GetFileName(subfolder);

		//	//	if (board == "hayden" || board == "server" || board == "temp")
		//	//		continue;

		//	//	var baseDirectory = Path.Combine(Config.Value.FileLocation, board);
		//	//	Directory.CreateDirectory(Path.Combine(baseDirectory, "image"));
		//	//	Directory.CreateDirectory(Path.Combine(baseDirectory, "thumb"));
		//	//	Directory.CreateDirectory(Path.Combine(baseDirectory, "thread"));

		//	//	foreach (var jsonFile in Directory.EnumerateFiles(subfolder, "thread.json", SearchOption.AllDirectories))
		//	//	{
		//	//		if (jsonFile.StartsWith(Config.Value.FileLocation))
		//	//			continue;

		//	//		var thread = ReadJson(jsonFile);

		//	//		var existingThread = await dbContext.Threads.FirstOrDefaultAsync(x => x.Board == board && x.ThreadId == thread.OriginalPost.PostNumber);

		//	//		var lastModifiedTime = thread.OriginalPost.ArchivedOn.HasValue
		//	//			? Utility.ConvertGMTTimestamp(thread.OriginalPost.ArchivedOn.Value).UtcDateTime
		//	//			: Utility.ConvertGMTTimestamp(thread.Posts.Max(x => x.UnixTimestamp)).UtcDateTime;

		//	//		if (existingThread != null)
		//	//		{
		//	//			existingThread.IsDeleted = thread.IsDeleted == true;
		//	//			existingThread.IsArchived = thread.OriginalPost.Archived == true;
		//	//			existingThread.LastModified = lastModifiedTime;
		//	//		}
		//	//		else
		//	//		{
		//	//			existingThread = new DBThread()
		//	//			{
		//	//				Board = board,
		//	//				ThreadId = thread.OriginalPost.PostNumber,
		//	//				Title = thread.OriginalPost.Subject,
		//	//				IsArchived = thread.OriginalPost.Archived == true,
		//	//				IsDeleted = thread.OriginalPost.FileDeleted == true,
		//	//				LastModified = lastModifiedTime
		//	//			};

		//	//			dbContext.Add(existingThread);
		//	//		}

		//	//		var existingPosts = await dbContext.Posts.Where(x => x.Board == board && x.ThreadId == thread.OriginalPost.PostNumber).ToArrayAsync();

		//	//		foreach (var post in thread.Posts)
		//	//		{
		//	//			if (post == null)
		//	//				System.Diagnostics.Debugger.Break();

		//	//			var existingPost = existingPosts.FirstOrDefault(x => x.PostId == post.PostNumber);

		//	//			if (existingPost != null)
		//	//			{
		//	//				existingPost.IsDeleted = post.ExtensionIsDeleted == true;
		//	//				existingPost.Html = post.Comment;
		//	//				existingPost.IsImageDeleted = post.FileDeleted == true;
		//	//			}
		//	//			else
		//	//			{
		//	//				existingPost = new DBPost()
		//	//				{
		//	//					Board = board,
		//	//					PostId = post.PostNumber,
		//	//					ThreadId = thread.OriginalPost.PostNumber,
		//	//					Html = post.Comment,
		//	//					Author = post.Name == "Anonymous" ? null : post.Name,
		//	//					MediaHash = post.FileMd5 == null ? null : Convert.FromBase64String(post.FileMd5),
		//	//					MediaFilename = post.OriginalFilenameFull,
		//	//					DateTime = Utility.ConvertGMTTimestamp(post.UnixTimestamp).UtcDateTime,
		//	//					IsSpoiler = post.SpoilerImage == true,
		//	//					IsDeleted = post.ExtensionIsDeleted == true,
		//	//					IsImageDeleted = post.FileDeleted == true
		//	//				};

		//	//				// This block of logic is to fix a bug with JSON files specifying the same posts multiple times
		//	//				var trackedPost = dbContext.Posts.Local.FirstOrDefault(x => x.Board == board && x.PostId == post.PostNumber);

		//	//				if (trackedPost != null)
		//	//				{
		//	//					dbContext.Entry(trackedPost).State = EntityState.Detached;
		//	//				}

		//	//				dbContext.Add(existingPost);
		//	//			}

		//	//			if (existingPost.MediaHash == null)
		//	//				continue;

		//	//			var base64Name = Utility.ConvertToBase(existingPost.MediaHash);

		//	//			if (string.IsNullOrWhiteSpace(base64Name))
		//	//				System.Diagnostics.Debugger.Break();

		//	//			var destinationFilename = Path.Combine(Config.Value.FileLocation, board, "image",
		//	//				base64Name + post.FileExtension);

		//	//			if (!System.IO.File.Exists(destinationFilename))
		//	//			{
		//	//				var sourceFilename = Path.Combine(subfolder, existingPost.ThreadId.ToString(),
		//	//					post.TimestampedFilenameFull);

		//	//				System.IO.File.Copy(sourceFilename, destinationFilename);



		//	//				sourceFilename = Path.Combine(subfolder, existingPost.ThreadId.ToString(), "thumbs",
		//	//					post.TimestampedFilename.Value + "s.jpg");

		//	//				destinationFilename = Path.Combine(Config.Value.FileLocation, board, "thumb",
		//	//					base64Name + ".jpg");

		//	//				System.IO.File.Copy(sourceFilename, destinationFilename);
		//	//			}
		//	//		}

		//	//		await dbContext.SaveChangesAsync();
		//	//		dbContext.DetachAllEntities();

		//	//		System.IO.File.Copy(jsonFile, Path.Combine(Config.Value.FileLocation, board, "thread", thread.OriginalPost.PostNumber + ".json"), true);
		//	//	}

		//	//	//foreach (var threadFolder in Directory.EnumerateDirectories(subfolder, "*", SearchOption.TopDirectoryOnly))
		//	//	//{
		//	//	//	IEnumerable<string> mediaFiles = Directory
		//	//	//						 .EnumerateFiles(threadFolder, "*.jpg" , SearchOption.TopDirectoryOnly)
		//	//	//		.Concat(Directory.EnumerateFiles(threadFolder, "*.jpeg", SearchOption.TopDirectoryOnly))
		//	//	//		.Concat(Directory.EnumerateFiles(threadFolder, "*.png" , SearchOption.TopDirectoryOnly))
		//	//	//		.Concat(Directory.EnumerateFiles(threadFolder, "*.webm", SearchOption.TopDirectoryOnly));

		//	//	//	foreach (var mediaFile in mediaFiles)
		//	//	//	{

		//	//	//	}
		//	//	//}
		//	//}

		//	return Ok();
		//}
	}
}