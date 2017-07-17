using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Ews.Client;
using Ews.Common;
using Mongoose.Common;
using Mongoose.Common.Attributes;
using Mongoose.Process;

namespace SmartConnector.UtilityExtensions
{
    /// <summary>
    /// Walks the EWS tree strucutre and exports ValueItem data to a CSV file.
    /// </summary>
    [ConfigurationDefaults("EWS Discovery Processor", "Browses the EWS tree and exports data to a CSV file.")]
    public class EwsDiscoveryProcessor : Processor, IEwsEndpoint
    {
        #region EwsEndpoint
        [Required, Tooltip("Address of the EWS endpoint to connect to.")]
        public string EwsEndpoint { get; set; }
        #endregion
        #region UserName
        [Required, EncryptedString, Tooltip("EWS UserName to connect with.")]
        public string UserName { get; set; }
        #endregion
        #region Password
        [Required, EncryptedString, Tooltip("EWS password to connect with.")]
        public string Password { get; set; }
        #endregion

        #region StartingContainerItemId
        [Tooltip("Id of the ContainerItem where discovery should start.  Leave empty to start at the root.")]
        public string StartingContainerItemId { get; set; }
        #endregion

        #region MaxContainerDepth
        [DefaultValue(20), Tooltip("The maximum nested ContainerItem depth which will be traversed.")]
        public int MaxContainerDepth { get; set; }
        #endregion

        #region OutputFilePath
        private string _outputFilePath = null;
        [Required, Tooltip("Complete path of the CSV file which will be generated.  It will be overwrittne if it exists.")]
        public string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                _outputFilePath = value;
                
                ResolvedOutputFilePath = string.IsNullOrEmpty(value) ?
                    string.Empty
                    : Environment.ExpandEnvironmentVariables(value);
            }
        }

        private string ResolvedOutputFilePath;
        #endregion

        #region IsLicensed - Override
        /// <inheritdoc />
        public override bool IsLicensed => false;
        #endregion

        #region Execute_Subclass - Override
        protected override IEnumerable<Prompt> Execute_Subclass()
        {
            // Delete the file if it exists.
            if (File.Exists(ResolvedOutputFilePath)) File.Delete(ResolvedOutputFilePath);

            // Add a header
            AddHeaderToOutputFile();

            // Get an instance of the client
            var client = MongooseObjectFactory.Current.GetInstance<IManagedEwsClient>();

            // Export from the starting point.  
            return ExportData(StartingContainerItemId, client) ?
                new List<Prompt>() :
                new List<Prompt> { new Prompt { Message = "Recursion terminated because maximum depth was reached.  Output is not complete", Severity = PromptSeverity.MayContinue } };
        }
        #endregion
        #region AddHeaderToOutputFile
        private void AddHeaderToOutputFile()
        {
            File.AppendAllText(ResolvedOutputFilePath, "ValueItemId,Name,Description,Type,Value,Forceable,Writeable,State,Unit");
        }
        #endregion
        #region ExportData
        private bool ExportData(string containerItemId, IManagedEwsClient client, int currentDepth = 0)
        {
            // Have we gone too deep?
            if (currentDepth > MaxContainerDepth) return false;

            var response = client.GetContainerItems(this, new[] { containerItemId });
            if (response.GetContainerItemsErrorResults != null && response.GetContainerItemsErrorResults.Length > 0)
            {
                // We'll log it but continue on.  Some stuff isn't traverseable in SBO
                SxL.Common.Logger.LogDebug(LogCategory.Testing, $"Couldn't retrieve ContainerItem {containerItemId}, server says {response.GetContainerItemsErrorResults.First().Message}");
                return true;
            }

            var containerItem = response.GetContainerItemsItems[0];

            // Export ValueItems
            foreach (var vi in containerItem?.Items?.ValueItems)
            {
                File.AppendAllText(ResolvedOutputFilePath, $"{vi.Id},{vi.Name},{vi.Description},{vi.Type},,{vi.Forceable},{vi.Writeable},,{vi.Unit}{Environment.NewLine}");
                CheckCancellationToken();
            }

            // Recurse over all child ContainerItems
            foreach (var ci in containerItem?.Items?.ContainerItems)
            {
                if (!ExportData(ci.Id, client, currentDepth + 1)) return false;
                CheckCancellationToken();
            }
            return true;
        }
        #endregion
    }
}
