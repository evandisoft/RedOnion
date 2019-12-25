function inf()
    for i=1,10000000 do
        print(i)
    end
    --error("blah")
end

function inf2()
    for i=1,100 do
        pcall(inf)
        print("after error")
        --sleep(1)
    end
    print("all done")
end

pcall(inf2)

print("past")