solution_file = "PageTypeBuilder.sln"
configuration = "release"
mspec_path = "packages/Machine.Specifications-Signed.0.4.24.0/tools/mspec-clr4.exe"
short_release_name = env("releasename")
release_name = "PageTypeBuilder-" + short_release_name
release_dir = "build"
release_versiondir = release_dir + "/" + release_name

target default, (compile):
  pass

target release, (compile, generate_specs, deploy, package, nuget):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)

desc "Build NuGet packages"
target nuget:
 
  exec("packages/NuGet.CommandLine.1.5.21005.9019/tools/nuget.exe", "pack \"PageTypeBuilder\\PageTypeBuilder.csproj\" -Prop Configuration=Release -Verbose /o ${release_dir}")  
  exec("packages/NuGet.CommandLine.1.5.21005.9019/tools/nuget.exe", "pack \"PageTypeBuilder.Migrations\\PageTypeBuilder.Migrations.csproj\" -Prop Configuration=Release -Verbose /o ${release_dir}")  
  exec("packages/NuGet.CommandLine.1.5.21005.9019/tools/nuget.exe", "pack \"PageTypeBuilder.Activation.StructureMap\\PageTypeBuilder.Activation.StructureMap.csproj\" -Prop Configuration=Release -Verbose /o ${release_dir}")

desc "Copies the binaries to the 'build' directory"
target deploy:
  print "Copying to build dir"

  with FileList("PageTypeBuilder/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

  with FileList("PageTypeBuilder.Activation.StructureMap/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

  with FileList("PageTypeBuilder.Migrations/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

desc "Generating specifications"
target generate_specs:
  mkdir(release_versiondir)
  exec(mspec_path, "--html ${release_versiondir}/specifications.html --silent PageTypeBuilder.Specs/bin/${configuration}/PageTypeBuilder.Specs.dll")
  rmdir("${release_versiondir}/resources")

desc "Creates zip package"
target package:
  zip("build", release_dir + "/" + release_name  +'.zip')
  exec("git archive master --format=zip > ${release_dir}/${release_name}-src.zip")

