function l()
    for i=1,100000 do
        print(i)
    end
end

c=coroutine.create(l)
retval,a=coroutine.resume(c)
print(coroutine.status(c))
print(retval,a)
print(3)