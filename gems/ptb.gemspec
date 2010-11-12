version = File.read(File.expand_path("../VERSION",__FILE__)).strip
Gem::Specification.new do |spec|
	spec.platform    = Gem::Platform::RUBY
	spec.name        = 'pagetypebuilder'
	spec.version     = version
	spec.files = Dir['lib/**/*'] + Dir['docs/**/*']
	spec.add_dependency('castle.core','= 2.5.1.0')
	spec.summary     = 'Page Type Builder'
	spec.description = 'Page Type Builder allows developers to define EPiServer page types in code which eliminates the need to synchronize page types between different servers. As page types are declared in code it also enables inheritance between page types and strongly typed property access.'
	spec.author = 'Joel Abrahamsson'
	spec.email             = 'mail@joelabrahamsson.com'
	spec.homepage          = 'http://pagetypebuilder.codeplex.com'
	spec.rubyforge_project = 'pagetypebuilder'
end