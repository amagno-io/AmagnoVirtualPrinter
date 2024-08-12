# Define the new version numbers
$newVersion = Read-Host "Enter the new version (e.g., 2.0.0.0)"

# Use the current directory as the root directory
$rootDirectory = Get-Location

# Get all AssemblyInfo.cs files in the directory and subdirectories
$assemblyInfoFiles = Get-ChildItem -Path $rootDirectory -Recurse -Filter "AssemblyInfo.cs"

$newAssemblyVersion = 'AssemblyVersion("' + $newVersion + '")';
$newFileVersion = 'AssemblyFileVersion("' + $newVersion + '")';

foreach ($file in $assemblyInfoFiles) {
    Write-Host "Processing $($file.FullName)"

    # Read the contents of the AssemblyInfo.cs file
    $assemblyInfoContent = Get-Content $file.FullName

    # Update the AssemblyVersion
    $assemblyInfoContent = $assemblyInfoContent -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $newAssemblyVersion

    # Update the AssemblyFileVersion
    $assemblyInfoContent = $assemblyInfoContent -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $newFileVersion

    # Write the updated content back to the AssemblyInfo.cs file
    Set-Content $file.FullName -Value $assemblyInfoContent

    Write-Host "Updated $($file.FullName)"
}

Write-Host "All AssemblyVersion and AssemblyFileVersion updates completed successfully."
