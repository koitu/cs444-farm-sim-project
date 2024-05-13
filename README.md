# Farm Sim
```
adb exec-out "while true; do screenrecord --bit-rate=1m --output-format=h264 --time-limit 180 -; done" | vlc --demux h264 --h264-fps=60 --clock-jitter=0 -
```

