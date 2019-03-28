function blah()
    print("yielding")
    coroutine.yield()
end

for i=1,10 do
    blah()
end
print("done")
return 1