$packagetitle = "Mkay.0.0.1"
$targetdir = "Mkay.Nuget/" + $packagetitle
$jsdir = $targetdir + "/content/Scripts"
$dlldir = $targetdir + "/lib/net40"

New-Item $jsdir -ItemType Directory -ea 0 | Out-Null
New-Item $dlldir -ItemType Directory -ea 0 | Out-Null

Copy-Item Mkay.JS/Scripts/*.js $jsdir -force
Copy-Item Mkay/bin/debug/Mkay.dll $dlldir -force
