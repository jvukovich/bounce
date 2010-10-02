def mark_it_down!
	system 'perl docs/Markdown.pl README.markdown > docs/README.html'
end

mark_it_down!

watch 'README\.markdown' do
	mark_it_down!
end
