using ExifLibrary;
using rmSharp;

DB.sqLiteFile = "US_Presidents.rmTree";

using (var db = new DB())
{
    foreach (var entry in db.MultimediaTable)
    {
        if (File.Exists(entry.Filename))
        {
            try // ignore errors for files with wrong type (pdf, videos...) 
            {
                var img = ImageFile.FromFile(entry.Filename);

                var x = img.Properties.Get(ExifTag.WindowsTitle);

                var description = img.Properties.Get(ExifTag.WindowsTitle)?.Value as String;
                if (description != null)
                    entry.Caption = description; // overwrite caption

                var subject = img.Properties.Get(ExifTag.WindowsComment)?.Value as String;
                if (subject != null)
                    entry.Description = subject; // overwrite description                
            }
            catch { };
        }        
    }
    db.SaveChanges();
}






