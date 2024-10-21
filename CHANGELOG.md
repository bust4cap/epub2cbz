## v2024.10.21-1
- Fixed bug related to chapters and removing duplicate covers that prevented conversion from completing
- Rearranged settings window

## v2024.10.11-1
- Fixed rare case where Image Resolution info would be missing from ComicInfo

## v2024.10.10-1
- Adjusted duplicate cover detection
- Added Image Resolution to ComicInfo
- Fixed rare case where Double Page info would be missing from ComicInfo
- Added Producers/Editors and Translators to ComicInfo
- Added option to keep higher resolution Cover if two should be present

## v2024.10.09-1
- Added basic update check to settings window

## v2024.10.07-1
- Fixed proper page alignment in more cases

## v2024.10.06-1
- Fixed small issue for apps that aren't following the current ComicInfo schema
- Slightly adjusted translation

## v2024.09.30-1
- Added Abort button (currently working threads need to finish first)
- Added option to manually select how many CPU threads should be used

## v2024.09.28-1
- Increased search radius for table of contents from 4 to 6 pages (may need further adjustment in the future)
- Fixed Page Spread alignment in certain edge cases for books with wide images
- Slightly adjusted volume number detection in file names

## v2024.09.18-1
- Added support for multiple authors
- If a book has several languages set, prioritize English first, then Japanese, then whatever is listed first
- Language Code in the ComicInfo will now always be two letters
- Made some experimental features clearer

## v2024.09.16-1
- Important Bugfix
- Added Page Count to ComicInfo
- Added option to not include Chapters in ComicInfo

## v2024.09.15-1
- Added CBZ Compression Level options

## v2024.09.12-1
- Reverted index changes which caused KCC compatibility issues

## v2024.09.11-1
- Added page prefix "p_" to image filenames and "cover_" to the first image
- Start image filenames and ComicInfo index at 1 instead of 0
- Detecting earlier if a .cbz already exists

## v2024.09.09-1
- Added Double Page info to ComicInfo
- Rearranged position of ComicInfo items

## v2024.09.06-1
- Made overall conversion progress clearer in the text window

## v2024.09.05-1
- Small Bugfix related to wide image handling
- Small changes under the hood

## v2024.09.04-1
- Small Optimizations related to wide image handling
- Small Bugfix related to naming of extracted images

## v2024.09.03-3
- Small Bugfix related to Covers

## v2024.09.03-2
- Memory Optimizations

## v2024.09.03-1
- Added ability to directly convert to .cbz without having to extract files first
