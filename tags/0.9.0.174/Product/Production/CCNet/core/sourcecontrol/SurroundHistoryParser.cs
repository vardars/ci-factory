/********************************************************************************
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 ********************************************************************************/

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Implementation of IHistoryParser to handle Surround SCM output that describes modifications within the version control system.
	/// Format of output is:
	/// total-#
	/// {History Line}
	///
	/// where
	/// {History Line} has the following format:
	/// <repository><filename><rev><action><timestamp><comment><username><user email>
	/// </summary>
	public class SurroundHistoryParser : IHistoryParser
	{
		public const string TO_SSCM_DATE_FORMAT = "yyyyMMddHHmmss";

		public Modification[] Parse(TextReader sscmLog, DateTime from, DateTime to)
		{
			string line = sscmLog.ReadLine();
			int totalLines = int.Parse(line.Split('-')[1]);

			ArrayList modList = new ArrayList(totalLines);
			for (int i = 0; i < totalLines; i++)
			{
				line = sscmLog.ReadLine();
				modList.Add(ParseModificationLine(line));
			}
			return (Modification[]) modList.ToArray(typeof (Modification));
		}

		private Modification ParseModificationLine(string line)
		{
			Match match = Regex.Match(line, @"^<([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)>$");
			if (!match.Success)
			{
				throw new ArgumentException("Unable to parse line: " + line);
			}
			Modification modification = new Modification();
			modification.FolderName = match.Groups[1].ToString();
			modification.FileName = match.Groups[2].ToString();
			modification.ChangeNumber = Int32.Parse(match.Groups[3].ToString());
			modification.Type = match.Groups[4].ToString();
			modification.ModifiedTime = DateTime.ParseExact(match.Groups[5].ToString(), TO_SSCM_DATE_FORMAT, CultureInfo.InvariantCulture);
			modification.Comment = match.Groups[6].ToString();
			modification.UserName = match.Groups[7].ToString();
			modification.EmailAddress = match.Groups[8].ToString();
			return modification;
		}
	}
}