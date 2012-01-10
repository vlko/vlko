param($installPath, $toolsPath, $package, $project)

$package.GetFiles | Foreach-Object { write-host "file: $_" }
