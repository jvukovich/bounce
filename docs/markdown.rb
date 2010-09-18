watch 'README\.markdown' do
	puts 'marking down!'
	system 'perl docs/Markdown.pl README.markdown > docs/README.html'
end
