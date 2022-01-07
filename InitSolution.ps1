$launchFiles = Get-ChildItem -Path . -Recurse -Include launchSettings.default.json
Foreach ($file in $launchFiles) { 
	$copyfile = $file.FullName.Replace("launchSettings.default.json", "launchSettings.json")
	"Launch Setting File: " + $file.FullName
	"To create File: " + $copyfile
	Copy-Item $file.FullName -Destination $copyfile
}
