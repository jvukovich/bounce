require 'fileutils'

describe 'multistage' do
    before :each do
        FileUtils.rm_rf('archive')
        FileUtils.rm_rf('c:\deployments\bigsolution')
        FileUtils.rm_rf('c:\deployments\install')
    end

    it 'service' do
        system('bounce Service /stage:archive').should be_true

        pushd('archive') do
            system('bounce Service /stage:remoteDeploy').should be_true
        end

        assert_service_deployed('one')
        assert_service_deployed('two')
    end

    it 'web' do
        system('bounce Web /stage:archive').should be_true

        pushd('archive') do
            system('bounce Web /stage:remoteDeploy').should be_true
        end

        assert_web_deployed('one', false)
        assert_web_deployed('two', true)
    end

    it 'web local' do
        system('bounce Web /stage:archive').should be_true

        pushd('archive') do
            system('bounce Web /stage:deploy /machine:one').should be_true
        end

        assert_web_deployed('one', true)
        assert_web_deployed('two', false)
    end

    it 'both' do
        system('bounce Web Service /stage:archive').should be_true

        pushd('archive') do
            system('bounce Web Service /stage:remoteDeploy').should be_true
        end

        assert_service_deployed('one')
        assert_service_deployed('two')
        assert_web_deployed('one', false)
        assert_web_deployed('two', true)
    end

    def assert_bounce_deployed(machine)
        File.directory?('archive\bounce').should be_true
    end

    def assert_service_deployed(machine)
        assert_bounce_deployed(machine)
        File.directory?('archive\service').should be_true
        File.file?(%{c:\\deployments\\install\\#{machine}\\service\\service.txt}).should be_true
    end

    def assert_web_deployed(machine, deployed)
        assert_bounce_deployed(machine)
        File.directory?('archive\web').should be_true
        File.file?(%{c:\\deployments\\install\\#{machine}\\web\\stuff.txt}).should == deployed
    end
end

def pushd(dir)
    cwd = Dir.pwd
    Dir.chdir(dir)
    begin
        yield
    ensure
        Dir.chdir(cwd)
    end
end
