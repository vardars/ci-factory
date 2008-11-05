namespace Zeta.WebSpider.Spider
{
	#region Using directives.
	// ----------------------------------------------------------------------

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Text;

	// ----------------------------------------------------------------------
	#endregion

	/////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Downloading a complete website.
	/// </summary>
	public class WebSiteDownloader :
		IDisposable
	{
		#region Public methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="WebSiteDownloader"/> 
		/// class.
		/// </summary>
		/// <param name="options">The options.</param>
		public WebSiteDownloader(
			WebSiteDownloaderOptions options )
		{
            this.SetOptions(options);
		}

        public void SetOptions(WebSiteDownloaderOptions options)
        {
            Trace.WriteLine(
                string.Format(
                    @"Constructing WebSiteDownloader for URI '{0}', destination folder path '{1}'.",
                    options.DownloadUri,
                    options.DestinationFolderPath));

            _settings = SpiderSettings.Restore(options.DestinationFolderPath);

            _settings.Options = options;
        }

		/// <summary>
		/// Performs the complete downloading (synchronously). 
		/// Does return only when completely finished or when an exception
		/// occured.
		/// </summary>
		public void Process()
		{
			string baseUrl =
				_settings.Options.DownloadUri.OriginalString.TrimEnd( '/' ).
					Split( '?' )[0];

			if ( _settings.Options.DownloadUri.AbsolutePath.IndexOf( '/' ) >= 0 &&
				_settings.Options.DownloadUri.AbsolutePath.Length > 1 )
			{
				baseUrl = baseUrl.Substring( 0, baseUrl.LastIndexOf( '/' ) );
			}

			// --

			// The URI that is configured to be the start URI.
			Uri baseUri = new Uri( baseUrl, UriKind.Absolute );

			// The initial seed.
			DownloadedResourceInformation seedInfo =
				new DownloadedResourceInformation(
					_settings.Options,
					@"/",
					_settings.Options.DownloadUri,
					baseUri,
					_settings.Options.DestinationFolderPath,
					_settings.Options.DestinationFolderPath,
					UriType.Content );

			// --

			// Add the first one as the seed.
			if ( !_settings.HasContinueDownloadedResourceInfos )
			{
				_settings.AddContinueDownloadedResourceInfos( seedInfo );
			}

			// 2007-07-27, Uwe Keim:
			// Doing a multiple looping, to avoid stack overflows.
			// Since a download-"tree" (i.e. the hierachy of all downloadable
			// pages) can get _very_ deep, process one part at a time only.
			// The state is already persisted, so we need to set up again at
			// the previous position.
			int index = 0;
			while ( _settings.HasContinueDownloadedResourceInfos )
			{
				// Fetch one.
				DownloadedResourceInformation processInfo =
					_settings.PopContinueDownloadedResourceInfos();

				Trace.WriteLine(
					string.Format(
						@"{0}. loop: Starting processing URLs from '{1}'.",
						index + 1,
						processInfo.AbsoluteUri.AbsoluteUri ) );

				// Process the URI, add any continue URIs to start
				// again, later.
				ProcessUrl( processInfo, 0 );

				index++;
			}

			Trace.WriteLine(
				string.Format(
					@"{0}. loop: Finished processing URLs from seed '{1}'.",
					index + 1,
					_settings.Options.DownloadUri ) );
		}

		/// <summary>
		/// Performs the complete downloading (asynchronously). 
		/// Return immediately. Calls the ProcessCompleted event
		/// upon completion.
		/// </summary>
		public void ProcessAsync()
		{
			processAsyncBackgroundWorker = new BackgroundWorker();

			processAsyncBackgroundWorker.WorkerSupportsCancellation = true;

			processAsyncBackgroundWorker.DoWork +=
				processAsyncBackgroundWorker_DoWork;
			processAsyncBackgroundWorker.RunWorkerCompleted +=
				processAsyncBackgroundWorker_RunWorkerCompleted;

			// Start.
			processAsyncBackgroundWorker.RunWorkerAsync();
		}

		/// <summary>
		/// Cancels a currently running asynchron processing.
		/// </summary>
		public void CancelProcessAsync()
		{
			if ( processAsyncBackgroundWorker != null )
			{
				processAsyncBackgroundWorker.CancelAsync();
			}
		}

		// ------------------------------------------------------------------
		#endregion

		#region Public events.
		// ------------------------------------------------------------------

		public class ProcessingUrlEventArgs :
			EventArgs
		{
			#region Public methods.

			/// <summary>
			/// Constructor.
			/// </summary>
			internal ProcessingUrlEventArgs(
				DownloadedResourceInformation uriInfo,
				int depth )
			{
				this.uriInfo = uriInfo;
				this.depth = depth;
			}

			#endregion

			#region Public properties.

			/// <summary>
			/// 
			/// </summary>
			public int Depth
			{
				get
				{
					return depth;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public DownloadedResourceInformation UriInfo
			{
				get
				{
					return uriInfo;
				}
			}

			#endregion

			#region Private variables.

			private readonly DownloadedResourceInformation uriInfo;
			private readonly int depth;

			#endregion
		}

		public delegate void ProcessingUrlEventHandler(
			object sender,
			ProcessingUrlEventArgs e );

		/// <summary>
		/// Called when processing an URL.
		/// </summary>
		public event ProcessingUrlEventHandler ProcessingUrl;

		// ------------------------------------------------------------------
		#endregion

		#region Asynchron processing.
		// ------------------------------------------------------------------

		private BackgroundWorker processAsyncBackgroundWorker = null;

		void processAsyncBackgroundWorker_DoWork(
			object sender,
			DoWorkEventArgs e )
		{
			try
			{
				Process();
			}
			catch ( StopProcessingException )
			{
				// Do nothing, just end.
			}
		}

		void processAsyncBackgroundWorker_RunWorkerCompleted(
			object sender,
			RunWorkerCompletedEventArgs e )
		{
			if ( ProcessCompleted != null )
			{
				ProcessCompleted( this, e );
			}
		}

		public delegate void ProcessCompletedEventHandler(
			object sender,
			RunWorkerCompletedEventArgs e );

		/// <summary>
		/// Called when the asynchron processing is completed.
		/// </summary>
		public event ProcessCompletedEventHandler ProcessCompleted;

		/// <summary>
		/// 
		/// </summary>
		private class StopProcessingException :
			Exception
		{
		}

		// ------------------------------------------------------------------
		#endregion

		#region IDisposable members.
		// ------------------------------------------------------------------

		~WebSiteDownloader()
		{
			////settings.Persist();
		}

		public void Dispose()
		{
			////settings.Persist();
		}

		// ------------------------------------------------------------------
		#endregion

		#region Private methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Process one single URI with a document behind (i.e. no
		/// resource URI).
		/// </summary>
		/// <param name="uriInfo">The URI info.</param>
		/// <param name="depth">The depth.</param>
		private void ProcessUrl(
			DownloadedResourceInformation uriInfo,
			int depth )
		{
			Trace.WriteLine(
				string.Format(
					@"Processing URI '{0}', with depth {1}.",
					uriInfo.AbsoluteUri.AbsoluteUri,
					depth ) );

            string ext = DownloadedResourceInformation.CorrectFileExtension(DownloadedResourceInformation.TryExtractFileExtension(uriInfo.AbsoluteUri));

            if (ext == ".html" && _settings.Options.MaximumLinkDepth >= 0 &&
				depth > _settings.Options.MaximumLinkDepth )
			{
				Trace.WriteLine(
					string.Format(
						@"Depth {1} exceeds maximum configured depth. Ending recursion " +
							@"at URI '{0}'.",
						uriInfo.AbsoluteUri.AbsoluteUri,
						depth ) );
			}
			else if ( depth > _maxDepth )
			{
				Trace.WriteLine(
					string.Format(
						@"Depth {1} exceeds maximum allowed recursion depth. " +
							@"Ending recursion at URI '{0}' to possible continue later.",
						uriInfo.AbsoluteUri.AbsoluteUri,
						depth ) );

				// Add myself to start there later.
				// But only if not yet process, otherwise we would never finish.
				if ( _settings.HasDownloadedUri( uriInfo ) )
				{
					Trace.WriteLine(
						string.Format(
							@"URI '{0}' was already downloaded. NOT continuing later.",
							uriInfo.AbsoluteUri.AbsoluteUri ) );
				}
				else
				{
					_settings.AddDownloadedResourceInfo( uriInfo );

					// Finished the function.

					Trace.WriteLine(
						string.Format(
							@"Added URI '{0}' to continue later.",
							uriInfo.AbsoluteUri.AbsoluteUri ) );
				}
			}
			else
			{
				// If we are in asynchron mode, periodically check for stopps.
				if ( processAsyncBackgroundWorker != null )
				{
					if ( processAsyncBackgroundWorker.CancellationPending )
					{
						throw new StopProcessingException();
					}
				}

				// --

				// Notify event sinks about this URL.
				if ( ProcessingUrl != null )
				{
					ProcessingUrlEventArgs e = new ProcessingUrlEventArgs(
						uriInfo,
						depth );

					ProcessingUrl( this, e );
				}

				// --

				if ( uriInfo.IsProcessableUri )
				{
					if ( _settings.HasDownloadedUri( uriInfo ) )
					{
						Trace.WriteLine(
							string.Format(
								@"URI '{0}' was already downloaded. Skipping.",
								uriInfo.AbsoluteUri.AbsoluteUri ) );
					}
					else
					{
						Trace.WriteLine(
							string.Format(
								@"URI '{0}' was not already downloaded. Processing.",
								uriInfo.AbsoluteUri.AbsoluteUri ) );

						if ( uriInfo.LinkType == UriType.Resource )
						{
							Trace.WriteLine(
								string.Format(
									@"Processing resource URI '{0}', with depth {1}.",
									uriInfo.AbsoluteUri.AbsoluteUri,
									depth ) );

							byte[] binaryContent;

							ResourceDownloader.DownloadBinary(
								uriInfo.AbsoluteUri,
								out binaryContent,
								_settings.Options );

							ResourceStorer storer =
								new ResourceStorer( _settings );

							storer.StoreBinary(
								binaryContent,
								uriInfo );

							_settings.AddDownloadedResourceInfo( uriInfo );
							_settings.PersistDownloadedResourceInfo( uriInfo );
						}
						else
						{
							Trace.WriteLine(
								string.Format(
									@"Processing content URI '{0}', with depth {1}.",
									uriInfo.AbsoluteUri.AbsoluteUri,
									depth ) );

							string textContent;
							string encodingName;
							Encoding encoding;
							byte[] binaryContent;

							ResourceDownloader.DownloadHtml(
								uriInfo.AbsoluteUri,
								out textContent,
								out encodingName,
								out encoding,
								out binaryContent,
								_settings.Options );

							ResourceParser parser = new ResourceParser(
								_settings,
								uriInfo,
								textContent );

							List<UriResourceInformation> linkInfos =
								parser.ExtractLinks();

							ResourceRewriter rewriter =
								new ResourceRewriter( _settings );
							textContent = rewriter.ReplaceLinks(
								textContent,
								uriInfo );

							ResourceStorer storer =
								new ResourceStorer( _settings );

							storer.StoreHtml(
								textContent,
								encoding,
								uriInfo );

							// Add before parsing childs.
							_settings.AddDownloadedResourceInfo( uriInfo );

							foreach ( UriResourceInformation linkInfo in linkInfos )
							{
								DownloadedResourceInformation dlInfo =
									new DownloadedResourceInformation(
										linkInfo,
										uriInfo.LocalFolderPath,
										uriInfo.LocalBaseFolderPath );

								// Recurse.
								ProcessUrl( dlInfo, depth + 1 );

								// Do not return or break immediately if too deep, 
								// because this would omit certain pages at this
								// recursion level.
							}

							// Persist after completely parsed childs.
							_settings.PersistDownloadedResourceInfo( uriInfo );
						}

						Trace.WriteLine(
							string.Format(
								@"Finished processing URI '{0}'.",
								uriInfo.AbsoluteUri.AbsoluteUri ) );
					}
				}
				else
				{
					Trace.WriteLine(
						string.Format(
							@"URI '{0}' is not processable. Skipping.",
							uriInfo.AbsoluteUri.AbsoluteUri ) );
				}
			}
		}

		// ------------------------------------------------------------------
		#endregion

		#region Private variables.
		// ------------------------------------------------------------------

		private const int _maxDepth = 500;
		private SpiderSettings _settings = new SpiderSettings();

		// ------------------------------------------------------------------
		#endregion
	}

	/////////////////////////////////////////////////////////////////////////
}