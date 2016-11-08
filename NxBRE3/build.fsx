#r "./packages/FAKE/tools/FakeLib.dll"

open Fake


// Project information.
let authors = ["david@dossot.net"]
let projectName = "NxBRE"
let projectDescription = "Lightweight Business Rules Engine for .NET"
let projectSummary = projectDescription


// Tools.

// TODO: Would be better if this was restored into project.
let nugetPath = ProgramFilesX86 @@ "NuGet/nuget.exe"


// Directories.
let buildDir  = "./build/"
let deployDir = "./deploy/"
let packagingDir = "./packaging/"


// Filesets.
let appReferences = !! "/**/NxBRE.csproj"


// Targets.

Target "Clean" (fun _ -> CleanDirs [buildDir; deployDir; packagingDir])

Target "Build" (fun _ ->
    MSBuildDebug buildDir "Build" appReferences |> Log "AppBuild-Output: ")

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*") -- "*.zip"
        |> Zip buildDir (deployDir + "NxBRE.zip"))

Target "CreatePackage" (fun _ ->

    let net451Dir = packagingDir @@ "lib/net451/"
    CleanDirs [net451Dir]

    CopyFile net451Dir (buildDir @@ "NxBRE.dll")
    CopyFile net451Dir (buildDir @@ "NxBRE.pdb")
    CopyFile packagingDir "README.txt"
    CopyFile packagingDir "MIT-LICENSE.txt"

    let version = (net451Dir @@ "NxBRE.dll") |> GetAssemblyVersionString

    NuGet
        (fun p ->
            { p with
                Authors = authors
                Project = projectName
                Description = projectDescription
                OutputPath = packagingDir
                Summary = projectSummary
                WorkingDir = packagingDir
                Version = version
                Publish = false
                ToolPath = nugetPath
            })
        "NxBRE.nuspec"
)


// Build order.
"Clean" ==> "Build" ==> "Deploy"
"Clean" ==> "Build" ==> "CreatePackage"


// Start build.
RunTargetOrDefault "Build"
