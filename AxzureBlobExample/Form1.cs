using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;
using System.Diagnostics;

namespace AxzureBlobExample {
    public partial class Form1 : Form {

        private string _tag = null;

        public Form1() {
            InitializeComponent();
        }

        private async void cmdUpload_Click(object sender, EventArgs e) {

            using (var ofd = new OpenFileDialog()) {

                ofd.Filter = "PNG Files (*.png)|*.png";
                ofd.ShowDialog();

                CloudBlockBlob blockBlob = await GetBlockBlob();

                using (var fs = File.OpenRead(ofd.FileName)) {

                    var ac = 
                        _tag == null ? 
                        AccessCondition.GenerateIfNoneMatchCondition("*") : 
                        AccessCondition.GenerateIfMatchCondition(_tag);

                    await blockBlob.UploadFromStreamAsync(fs, ac, null, null);

                }

            }

            MessageBox.Show("OK");

        }

        private async void cmdDownload_Click(object sender, EventArgs e) {

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = await GetBlockBlob();

            using (var sfd = new SaveFileDialog()) {

                sfd.Filter = "PNG Files (*.png)|*.png";
                sfd.ShowDialog();

                await blockBlob.DownloadToFileAsync(sfd.FileName, FileMode.Create);
                _tag = blockBlob.Properties.ETag;

                Process.Start(sfd.FileName);

            }

        }

        private async Task<CloudBlockBlob> GetBlockBlob() {

            // Retrieve storage account from connection string.
            var setting = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(setting);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            // Retrieve reference to a blob named "myblob".
            return container.GetBlockBlobReference("myblob");

        }

    }
}
