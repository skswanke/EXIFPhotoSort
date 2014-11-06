using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string[] validExtentions = new string[4] {".jpg", ".JPG", ".jpeg", ".JPEG"};

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int fileSkipCount = 0;
            bool errors = false;
            button1.Enabled = false;
            fileDialogButton.Enabled = false;
            bool copyFiles = checkBox1.Checked;
            if (this.textBox1.Text == ""||this.textBox1.Text.Contains("Files"))
            {
                MessageBox.Show("You need to Select a file first!", "Message");
            }
            else
            {
                string[] files = Directory.GetFiles(this.textBox1.Text);
                progressBar1.Maximum = files.Length;

                try
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        int propNum = 0;
                        bool validProp = false;
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        progressBar1.Value += 1;
                        
                        try 
                        {
                            if (validExtentions.Contains(Path.GetExtension(files[i].ToString())))
                            {
                                //Load image
                                Image theImage = new Bitmap(@files[i].ToString());
                                Console.WriteLine("Image Loaded");
                                Console.WriteLine(files[i].ToString());

                                //Get Exif
                                PropertyItem[] propItems = theImage.PropertyItems;
                                Console.WriteLine("Property Items Found");

                                //Clear Ref
                                theImage.Dispose();
                                Console.WriteLine("Image Ref Disposed");
                         
                                //Validate Properties
                                while (!validProp && propNum < 15)
                                {
                                    string dateTaken = encoding.GetString(propItems[propNum].Value);
                                    
                                    //Check Items
                                    if (propItems.Length == 0)
                                    {
                                        errors = true;
                                        fileSkipCount += 1;
                                        Console.WriteLine("propItems Length = 0");
                                        validProp = true;
                                    }

                                    //Check that the items are there
                                    else if (propItems.Length < 7)
                                    {
                                        errors = true;
                                        fileSkipCount += 1;
                                        Console.WriteLine("propItems Length < 7");
                                        validProp = true;
                                    }

                                    //Check that the desired prop is there
                                    else if (propItems[propNum] == null || dateTaken.Length > 30)
                                    {
                                        Console.WriteLine("propItems[" + propNum + "] was null");
                                        propNum += 1;
                                    }

                                    //If it is make sure it is in calender format
                                    else if (!dateTaken.Contains(':'))
                                    {
                                        Console.WriteLine("propItems[" + propNum + "] was not a date");
                                        propNum += 1;
                                    }

                                    //Sort into new folders
                                    else
                                    {
                                        Console.WriteLine("DateTaken was Found!");            
                                        string originPath = files[i].ToString();
                                        string newPath = this.textBox1.Text + '\\' + dateTaken.Substring(0, 4) + '_' + dateTaken.Substring(5, 2) + '\\' + Path.GetFileName(files[i].ToString());
                                        string newFolder = this.textBox1.Text + '\\' + dateTaken.Substring(0, 4) + '_' + dateTaken.Substring(5, 2) + '\\';
                                        if (!Directory.Exists(newFolder))
                                        {
                                            Directory.CreateDirectory(newFolder);
                                            Console.WriteLine("New Directory Was Created");
                                        }

                                        //Move or Copy Files
                                        if (!checkBox1.Checked)
                                        {
                                            File.Move(files[i].ToString(), this.textBox1.Text + '\\' + dateTaken.Substring(0, 4) + '_' + dateTaken.Substring(5, 2) + '\\' + Path.GetFileName(files[i].ToString()));
                                            Console.WriteLine("Files were Moved");
                                        }
                                        else
                                        {
                                            File.Copy(files[i].ToString(), this.textBox1.Text + '\\' + dateTaken.Substring(0, 4) + '_' + dateTaken.Substring(5, 2) + '\\' + Path.GetFileName(files[i].ToString()));
                                            Console.WriteLine("Files were Copied");
                                        }
                                        validProp = true;
                                    }
                                }
                                //Make sure fileskip count counts files that went through 15 checks
                                if (!validProp)
                                    fileSkipCount += 1;
                            }

                            //If the extention was not Valid
                            else
                            {
                                Console.WriteLine("A file was Skipped");
                                errors = true;
                                fileSkipCount += 1;
                                validProp = true;
                            }
                        }

                        //General Catch
                        catch (Exception)
                        {
                            Console.WriteLine("File Skipped Due to Error");
                            fileSkipCount += 1;
                            continue;
                        }
                    }
                    //Tell the user if there were errors
                    if (errors)
                    {
                        MessageBox.Show("Items were skipped! (no Exif)\nFiles Skipped: " + fileSkipCount, "Message");
                    }
                    else
                    {
                        MessageBox.Show("Success!", "Message");
                    }
                    this.textBox1.Text = "Files Skipped: " + fileSkipCount.ToString();
                }
                
                catch (Exception)
                {
                    MessageBox.Show("Error", "Error");
                    throw;
                }
            }
            button1.Enabled = true;
            fileDialogButton.Enabled = true;
            progressBar1.Value = 0;
        }

        private void fileDialogButton_Click(object sender, EventArgs e)
        {
            DialogResult folderLocation = folderBrowserDialog1.ShowDialog();
            if (folderLocation == DialogResult.OK)
            {
                string[] files1 = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                MessageBox.Show("Files Found: " + files1.Length.ToString(), "Message");
                this.textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            Console.WriteLine(folderLocation);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
