﻿using System;
using Nest;

namespace Hayden.WebServer.DB.Elasticsearch
{
	[ElasticsearchType(RelationName = IndexName)]
	public class PostIndex
	{
		public const string IndexName = "post_index";
		
		//[Number(NumberType.UnsignedLong, Index = false, Store = false, DocValues = false)]
		//public ulong DocId { get; set; }
		//[Keyword(Index = false, Store = false, Similarity = "boolean")]
		//public string DocId { get; set; }

		[Number(NumberType.Integer, Name = "postId", Index = false, Store = false, DocValues = true)]
		public ulong PostId { get; set; }

		[Number(NumberType.Integer, Name = "threadId", Index = false, Store = false, DocValues = true)]
		public ulong ThreadId { get; set; }

		[Number(NumberType.Short, Name = "boardId", Index = false, Store = false, DocValues = true)]
		public ushort BoardId { get; set; }

		//[Text(Name = "postHtmlText", Index = true, Store = false, Similarity = "boolean")]
		//public string PostHtmlText { get; set; }
		
		[Text(Name = "postRawText", Index = true, Store = false, Similarity = "boolean")]
		//[MatchOnlyText(Name = "postRawText")] // , Store = false, Similarity = "boolean"
		public string PostRawText { get; set; }

		//[Text(Name = "subject", Index = true, Store = false, Similarity = "boolean")]
		//public string Subject { get; set; }

		[Date(Name = "postDateUtc", Index = false, Store = false, DocValues = true)]
		public DateTime PostDateUtc { get; set; }

		[Boolean(Name = "isOp", Index = true, Store = false, DocValues = true)]
		public bool IsOp { get; set; }
	}
}
