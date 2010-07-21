module Nu
  class Project
  
    def initialize
      @config_file = ".nu/options.yaml"
    end

    def ensure_default_config
      if File.exist? @config_file
  			return
  		end
  		
  		content = YAML::dump( {'lib'=>'lib'} )
  		add_file @config_file, content 
    end
  
    def get_location
  		content = YAML.load_file @config_file
  		content['lib']
  	end
	
  	def set_location(name)
  	  File.delete @config_file if File.exist? @config_file

  		content = YAML::dump( { 'lib' => name })
		
  		File.open(@config_file, 'w') {|f| f.write(content) }
    end
    
    def add_file(name,content)
      File.open(name, 'w'){|f| f.write(content)}
    end
    
  end
end