# UnityCodeScanner
Unity app to scan codes using Tesseract OCR.
It works on iOS and Android (still to be tested)


## iOS
To make it work on iOS, remember to add `libz.tbd` to `UnityFramework.framework` on XCode
Also disable `Enable Bitcode` on `Build Settings` for both targets - App and Framework.
