function blah()
    print("yielding")
    coroutine.yield()
end

while true do
    blah()
end