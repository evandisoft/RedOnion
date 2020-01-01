require("basiclib")

countdown()

autopilot.pitch=90
autopilot.heading=0
ship.throttle=1

stage()

print('staged')
waitforfalling()
print('falling')


--print('running experiments')
--runexps()

stage()

print('waiting for parachutes')
autopara()
print('parachutes deployed')

autopilot.reset()