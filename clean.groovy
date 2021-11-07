def project = new File("/Users/avilches/Godot/Veronenger")
println project
new File(project, ".mono").deleteDir()
new File(project, "Tools/bin").deleteDir()
new File(project, "Tools/obj").deleteDir()
project.listFiles().findAll{ x -> x.name.startsWith("mono_crash") }.each { f -> println f.name; f.delete() }


