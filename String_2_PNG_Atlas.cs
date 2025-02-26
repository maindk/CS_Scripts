using Microsoft.VisualBasic.FileIO;
using System.Drawing.Drawing2D;

namespace String_to_png_to_atlas
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            //Sets the default Image size
            int imageHW = 512;
            
            //Sets the Images parameters
            Color bgColor = Color.Black;
            Color textColor = Color.White;
            SolidBrush textBrush = new SolidBrush(textColor);

            float fontSize = 60.0f;
            Font ourFont = new Font("Ariel", fontSize, FontStyle.Bold, GraphicsUnit.Point);
            StringFormat strAlignment = new StringFormat();
            strAlignment.LineAlignment = StringAlignment.Center;
            strAlignment.Alignment = StringAlignment.Center;


            //create a dialog for choosing a file;
            OpenFileDialog openfileDialog1 = new OpenFileDialog();
            openfileDialog1.InitialDirectory = "C:/";
            openfileDialog1.Filter = "CSV (*.csv)| *.csv";
            openfileDialog1.FilterIndex = 0;
            openfileDialog1.RestoreDirectory = true;

            //Datafile that you choose
            string dataFile;

            //Creats a dialog where you choose your database file.
            if (openfileDialog1.ShowDialog() != DialogResult.OK)
            {
                dataFile = openfileDialog1.FileName;
                return;
            }
            else 
            {
                dataFile = openfileDialog1.FileName;
            }

           TextFieldParser csvParser = new TextFieldParser(dataFile);

            //Creates a dialog where you can choose the folder destination.
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Choose your file directory folder where you want to save";
            folderBrowserDialog1.InitialDirectory = "C:/";
            folderBrowserDialog1.UseDescriptionForTitle = true;
            
            //opens the filder directory wehere you want to save in
            string openLocation;

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
            {
                openLocation = folderBrowserDialog1.SelectedPath;
            }
            else
            {
                openLocation = folderBrowserDialog1.SelectedPath;
            }
            
            //creates a directory for the images to be saved in
            DirectoryInfo di = Directory.CreateDirectory(openLocation + "/" + "images");

            //uses a parser and makes images
            using (csvParser)
            {
                csvParser.TextFieldType = FieldType.Delimited;
                csvParser.SetDelimiters(",");

                while (!csvParser.EndOfData)
                {
                    //grabs the strings
                    string[] lines = csvParser.ReadFields();

                    foreach (string doc in lines)
                    {
                        //creates bitmap 
                        Bitmap bmp = new Bitmap(imageHW, imageHW);
                        Image img = Image.FromHbitmap(bmp.GetHbitmap());

                        float xScaleFactor = .25f;
                        float yScaleFactor = .25f;

                        //Gets our graphics objext
                        Graphics g = Graphics.FromImage(img);
                        g.Clear(bgColor);

                        //Translates the image
                        g.TranslateTransform(imageHW / 2 , imageHW / 2);

                        //Scales the image
                        g.ScaleTransform(xScaleFactor, yScaleFactor);

                        //makes the Image
                        g.DrawString(doc, ourFont, textBrush, 0, 0, strAlignment);

                        g.Flush(FlushIntention.Sync);

                        //saves images
                        img.Save(di + "/" + doc + ".png");

                        //clean up
                        g.Dispose();
                        bmp.Dispose();
                    }
                }
            }

            //this atlas's your images you created
            DialogResult choosedialogResult = MessageBox.Show("Do you want to atlas your Images?", "Atlas Maker", MessageBoxButtons.YesNo);
            if (choosedialogResult == DialogResult.Yes)
            {
                ////Creates coords
                //int quantityOfImages = Directory.GetFiles(Atlas_Images).Length;
                //int totalVolumeOfImgs = quantityOfImages * (int)Math.Pow(imageHW, 2);

                //int sizeOfAtlas = 2048;

                //int xCoordinate = 0;
                //int yCoordinate = 0;

                //int[,] imageCoords = new int[quantityOfImages, 2];

                ////creates a scalable atlas
                //while ((int)Math.Pow(sizeOfAtlas, 2) <= totalVolumeOfImgs)
                //{
                //    sizeOfAtlas *= 2;
                //}

                ////creates coords
                //for (int i = 0; i < quantityOfImages; i++)
                //{
                //    if (xCoordinate + imageHW <= sizeOfAtlas; i++)

                //    {
                //    //return xCoordinate
                //    imageCoords[i, 0] = xCoordinate;
                //    //return yCoordinate
                //    imageCoords[i, 1] = yCoordinate;
                //    }

                //    else
                //{
                //    xCoordinate = 0;
                //    yCoordinate += imageHW;
                //    //return xCoordinate
                //    imageCoords[i, 0] = xCoordinate;
                //    //return yCoordinate
                //    imageCoords[i, 1] = yCoordinate;
                //}
                //xCoordinate += imageHW;




                ////creates bitmap to layer pics on for the atlas
                //Bitmap combBitmap = new Bitmap(sizeOfAtlas, sizeOfAtlas);

                ////Places all images in directory
                //for (int i = 0; i < quantityOfImages; i++)
                //{
                //    Image imageSet = Image.FromFile(Directory.GetFiles(Atlas_Images)[i]);

                //    using (Graphics g = Graphics.FromImage(combBitmap))
                //    {
                //        g.DrawImage(imageSet, imageCoords[i, 0], imageCoords[i, 1], imageHW, imageHW);
                //        g.Dispose();
                //    }
                //}
                //combBitmap.Save("Atlas.png");
            }
            else if (choosedialogResult == DialogResult.No)
            {
                MessageBox.Show("Files Opened from here: " + dataFile + "\n" + "" + "\n" + "Files Saved here: " + di, "Completed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //Makes a message box showing where you saved your files and what file you chose
            MessageBox.Show("Files Opened from here: " + dataFile + "\n" + "" + "\n" + "Files Saved here: " + di, "Completed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}