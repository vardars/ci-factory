namespace Zeta.WebSpider.Spider
{
	#region Using directives.
	// ----------------------------------------------------------------------

	using System;
	using System.Runtime.Serialization;
	using System.Collections.Generic;
	using System.IO;
	using System.Diagnostics;
	using System.Runtime.Serialization.Formatters.Binary;

	// ----------------------------------------------------------------------
	#endregion

	/////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Class to store as the persistent state of the downloader.
	/// </summary>
	[Serializable]
	public class SpiderSettings
	{
		#region Public methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Persistently stores this object.
		/// </summary>
		private void Persist()
		{
			try
			{
				string filePath = Path.Combine(
					_options.DestinationFolderPath.FullName,
					@"WebSiteDownloader.state" );

				if ( File.Exists( filePath ) )
				{
					File.Delete( filePath );
				}

				Trace.WriteLine(
					string.Format(
						@"About to persist spider settings to file '{0}'. " +
							@"{1} temporary downloaded resources, " +
								@"{2} persistent downloaded resources, " +
								@"{3} continue downloaded resources.",
						filePath,
						_temporaryDownloadedResourceInfos.Count,
						_persistentDownloadedResourceInfos.Count,
						_continueDownloadedResourceInfos.Count ) );

				BinaryFormatter serializer =
					new BinaryFormatter();
				using ( FileStream writer = new FileStream(
					filePath,
					FileMode.CreateNew,
					FileAccess.Write ) )
				{
					serializer.Serialize(
						writer,
						this );
				}
			}
			catch ( IOException x )
			{
				Trace.WriteLine(
					string.Format(
					@"Ignoring IO exception while persisting spider settings: '{0}'.",
					x.Message ) );
			}
			catch ( UnauthorizedAccessException x )
			{
				Trace.WriteLine(
					string.Format(
					@"Ignoring exception while persisting spider settings: '{0}'.",
					x.Message ) );
			}
		}

		/// <summary>
		/// Restore a previously stored setting value from the given
		/// folder path.
		/// </summary>
		/// <returns>Returns an empty object if not found.</returns>
		public static SpiderSettings Restore(
			DirectoryInfo folderPath )
		{
			string filePath = Path.Combine(
				folderPath.FullName,
				@"WebSiteDownloader.state" );

			if ( File.Exists( filePath ) )
			{
				try
				{
					BinaryFormatter serializer =
						new BinaryFormatter();
					using ( FileStream reader = new FileStream(
						filePath,
						FileMode.Open,
						FileAccess.Read ) )
					{
						SpiderSettings settings =
							(SpiderSettings)serializer.Deserialize( reader );

						settings.Options = new WebSiteDownloaderOptions();
						settings.Options.DestinationFolderPath = folderPath;

						if ( settings._temporaryDownloadedResourceInfos == null )
						{
							settings._temporaryDownloadedResourceInfos =
								new List<DownloadedResourceInformation>();
						}
						if ( settings._persistentDownloadedResourceInfos == null )
						{
							settings._persistentDownloadedResourceInfos =
								new List<DownloadedResourceInformation>();
						}
						if ( settings._continueDownloadedResourceInfos == null )
						{
							settings._continueDownloadedResourceInfos =
								new List<DownloadedResourceInformation>();
						}

						// Move from persistent storage back to memory.
						settings._temporaryDownloadedResourceInfos.Clear();
						settings._temporaryDownloadedResourceInfos.AddRange(
							settings._persistentDownloadedResourceInfos );

						Trace.WriteLine(
							string.Format(
								@"Successfully restored spider settings from file '{0}'. " +
									@"{1} temporary downloaded resources, " +
										@"{2} persistent downloaded resources, " +
											@"{3} continue downloaded resources.",
								filePath,
								settings._temporaryDownloadedResourceInfos.Count,
								settings._persistentDownloadedResourceInfos.Count,
								settings._continueDownloadedResourceInfos.Count
								) );

						return settings;
					}
				}
				catch ( SerializationException x )
				{
					Trace.WriteLine(
						string.Format(
						@"Ignoring exception while deserializing spider settings: '{0}'.",
						x.Message ) );

					SpiderSettings settings = new SpiderSettings();
					settings.Options.DestinationFolderPath = folderPath;

					return settings;
				}
				catch ( IOException x )
				{
					Trace.WriteLine(
						string.Format(
						@"Ignoring IO exception while loading spider settings: '{0}'.",
						x.Message ) );

					SpiderSettings settings = new SpiderSettings();
					settings.Options.DestinationFolderPath = folderPath;

					return settings;
				}
				catch ( UnauthorizedAccessException x )
				{
					Trace.WriteLine(
						string.Format(
						@"Ignoring exception while loading spider settings: '{0}'.",
						x.Message ) );

					SpiderSettings settings = new SpiderSettings();
					settings.Options.DestinationFolderPath = folderPath;

					return settings;
				}
			}
			else
			{
				SpiderSettings settings = new SpiderSettings();
				settings.Options.DestinationFolderPath = folderPath;

				return settings;
			}
		}

		/// <summary>
		/// Check whether a file was already downloaded.
		/// </summary>
		/// <param name="uriInfo">The URI info.</param>
		/// <returns>
		/// 	<c>true</c> if [has downloaded URI] [the specified URI info]; 
		/// otherwise, <c>false</c>.
		/// </returns>
		public bool HasDownloadedUri(
			DownloadedResourceInformation uriInfo )
		{
			// Search whether exists in list.
			int foundPosition =
				_temporaryDownloadedResourceInfos.IndexOf(
				uriInfo );

			if ( foundPosition < 0 )
			{
				return false;
			}
			else
			{
				// Found. Check various attributes.
				DownloadedResourceInformation foundInfo =
					_temporaryDownloadedResourceInfos[foundPosition];

				if ( foundInfo.AddedByProcessID ==
					Process.GetCurrentProcess().Id )
				{
					return true;
				}
				else if ( foundInfo.DateAdded.AddHours( 10 ) > DateTime.Now )
				{
					return true;
				}
				else
				{
					return foundInfo.FileExists;
				}
			}
		}

		/// <summary>
		/// Add information about a downloaded resource.
		/// </summary>
		/// <param name="info">The info.</param>
		public void AddDownloadedResourceInfo(
			DownloadedResourceInformation info )
		{
			if ( _temporaryDownloadedResourceInfos.Contains( info ) )
			{
				_temporaryDownloadedResourceInfos.Remove( info );
			}

			_temporaryDownloadedResourceInfos.Add( info );
		}

		/// <summary>
		/// Persist information about a downloaded resource.
		/// </summary>
		/// <param name="uriInfo">The URI info.</param>
		public void PersistDownloadedResourceInfo(
			DownloadedResourceInformation uriInfo )
		{
			int foundPosition =
				_temporaryDownloadedResourceInfos.IndexOf(
				uriInfo );

			DownloadedResourceInformation foundInfo =
				_temporaryDownloadedResourceInfos[foundPosition];

			// --

			// Move over.
			if ( _persistentDownloadedResourceInfos.Contains( foundInfo ) )
			{
				_persistentDownloadedResourceInfos.Remove( foundInfo );
			}

			_persistentDownloadedResourceInfos.Add( foundInfo );

			// And store.
			Persist();
		}

		/// <summary>
		/// The URLs where to continue parsing when the stack trace gets too deep.
		/// </summary>
		/// <value>The continue downloaded resource infos.</value>
		public void AddContinueDownloadedResourceInfos(
			DownloadedResourceInformation resourceInfo )
		{
			if ( _continueDownloadedResourceInfos.Contains( resourceInfo ) )
			{
				_continueDownloadedResourceInfos.Remove( resourceInfo );
			}

			_continueDownloadedResourceInfos.Add( resourceInfo );
			Persist();
		}

		/// <summary>
		/// Pops the continue downloaded resource infos.
		/// </summary>
		/// <returns>Returns the first entry or NULL if none.</returns>
		public DownloadedResourceInformation PopContinueDownloadedResourceInfos()
		{
			if ( _continueDownloadedResourceInfos.Count <= 0 )
			{
				return null;
			}
			else
			{
				DownloadedResourceInformation result =
					_continueDownloadedResourceInfos[0];

				_continueDownloadedResourceInfos.RemoveAt( 0 );

				Persist();

				return result;
			}
		}

		// ------------------------------------------------------------------
		#endregion

		#region Public properties.
		// ------------------------------------------------------------------

		/// <summary>
		/// The options.
		/// </summary>
		/// <value>The options.</value>
		public WebSiteDownloaderOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				_options = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has continue
		/// downloaded resource infos.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has continue downloaded resource
		/// infos; otherwise, <c>false</c>.
		/// </value>
		public bool HasContinueDownloadedResourceInfos
		{
			get
			{
				return _continueDownloadedResourceInfos.Count > 0;
			}
		}

		// ------------------------------------------------------------------
		#endregion

		#region Private variables.
		// ------------------------------------------------------------------

		[NonSerialized]
		private WebSiteDownloaderOptions _options =
			new WebSiteDownloaderOptions();

		[NonSerialized]
		private List<DownloadedResourceInformation> _temporaryDownloadedResourceInfos =
			new List<DownloadedResourceInformation>();

		private List<DownloadedResourceInformation> _persistentDownloadedResourceInfos =
			new List<DownloadedResourceInformation>();

		/// <summary>
		/// The URLs where to continue parsing when the stack trace gets too deep.
		/// </summary>
		private List<DownloadedResourceInformation> _continueDownloadedResourceInfos =
			new List<DownloadedResourceInformation>();

		// ------------------------------------------------------------------
		#endregion
	}

	/////////////////////////////////////////////////////////////////////////
}