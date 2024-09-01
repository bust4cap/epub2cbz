# epub2cbz
Python program to extract .epub Manga and Comic ebooks into "cbz-ready" folders, including a ComicInfo.xml file with chapter info, reading direction and more.

# Prerequisites
- Python 3
- "rich" (for colored text messages) and "PIL" (to be able to create blank images) for Python (install via: pip install rich pillow)
- epubs should ideally be in the format of "[Series] v[Volume].epub", for example "Two Pieces v69.epub" (both are optional, but are derived from the filename, if enabled)

# How to
- move all to be converted .epub files into one folder
- open a command line window in that folder
- type "python ", drag and drop the epub2cbz.py file onto the command line window and press return
- (TEST WITH ONE OR A FEW FILES FIRST)

- after the conversion (and if you have 7zip installed) you can use the following command in the same folders command line to turn all folders into .cbz files (adjust the path to your 7z.exe):
  FOR /F "usebackq delims=?" %i IN (\`DIR /B /A:D\`) DO C:\PATH\TO\7-Zip\7z.exe a -tzip "%i.cbz" "%i"
