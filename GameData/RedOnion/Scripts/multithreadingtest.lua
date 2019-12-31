print("waiting for sleep")
sleep(5)
if not somelock then
    somelock=true
    print("only one thread is in the locked area")
    somelock=false
end
print("thread done")