## v2025.05.03-1
- Switched from the native to a custom config file, so settings won't get reset between releases anymore

## v2025.05.01-1
- Fixed a small bug related to metadata parsing that could prevent conversion from completing when "Create ComicInfo.xml" was enabled

## v2025.04.26-1
- Sped up conversion significantly in certain scenarios
- Improved duplicate cover detection

## v2025.04.25-1
- Added WebP image support
- Fixed files not processing when "Create ComicInfo.xml" is unchecked
- Updated Framework from .NET 6 to .NET 9, resulting in reduced program size as well (Settings will be reset to default due to this change)
- Added optional basic DRM Check
- Slightly adjusted wide image detection. Images now need to be at least 10% wider than tall to be considered "wide"
- Error Popups now center on the main window

## v2025.04.16-1
- Window Location is now also saved between sessions

## v2025.04.15-2
- Fixed bug where certain settings were not properly saved

## v2025.04.15-1
- Fixed small bug related to the "Reset Settings" function
- Added 3 second timeout to the update check

## v2025.04.14-1
- Settings are now saved between sessions
- Added "Reset Settings" button to reset settings to their default recommended values

## v2025.04.09-1
- Added Japanese language support (Translation by: PickledCakes)
- Adjusted English translation slightly to be more clear
- Fixed Blank Page behavior in certain cases

## v2025.02.20-1
- Fixed adding the same author multiple times in certain cases
- Fixed ComicInfo compatibility with certain Programs in more cases related to publishers
- Fixed incorrect image extraction for manga of a certain publisher
- Adjusted how certain blocked symbols like ":" and "?" in file names are handled in the ComicInfo for the "Series" field
- Added tooltip for long folder paths

## v2024.12.27-2
- Fixed duplicate cover detection in more cases

## v2024.12.27-1
- Fixed image path detection in certain cases

## v2024.12.17-1
- Fixed date detection in certain cases where the date didn't follow the standard formatting

## v2024.11.02-2
- Fixed blank image handling in certain cases
- Increased tolerance for blank image detection

## v2024.11.02-1
- Added option to add an additional blank image if the Page Spread alignment info is assumed to be set incorrectly in the original Ebook (this option is disabled by default)
- Fixed duplicate cover removal for certain Japanese language books

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
