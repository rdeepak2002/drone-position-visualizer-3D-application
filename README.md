# drone-position-visualizer-3D-application

## About

Unity 3D application to view drone 3D position data received from a [server](https://github.com/rdeepak2002/drone-position-visualizer-server)

## Requirements

- [Unity 3D 2021.3.5f1](https://unity.com/releases/editor/whats-new/2021.3.5)
- [drone-position-visualizer-server](https://github.com/rdeepak2002/drone-position-visualizer-server)

## How to Run (Development)

```shell
git clone https://github.com/rdeepak2002/drone-position-visualizer-3D-application
```

- Open ``drone-position-visualizer-3D-application`` folder with Unity 3D 2021.3.5f1
- Ensure ``SocketManagerDev`` game object is enabled and ``SocketManagerProd`` game object is disabled
- Ensure ``Socket.IO Communicator (v3 and v4)`` script in ``SocketManagerDev`` game object has ``Socket IO Address`` set to ``localhost:8080``
- Ensure [drone-position-visualizer-server](https://github.com/rdeepak2002/drone-position-visualizer-server) is running at port 8080
- Ensure ``Socket Manager`` script in ``SocketManagerDev`` has ``Point Prefab`` variable referenced to "Assets/Scenes/MainScene/Point" game object or any game object of your choice
- Run the project

## How to Run (Production)

```shell
git clone https://github.com/rdeepak2002/drone-position-visualizer-3D-application
```

- Open ``drone-position-visualizer-3D-application`` folder with Unity 3D 2021.3.5f1
- Ensure ``SocketManagerProd`` game object is enabled and ``SocketManagerDev`` game object is disabled
- Ensure ``Socket.IO Communicator (v3 and v4)`` script in ``SocketManagerProd`` game object has ``Socket IO Address`` set to ``drone-position-visualizer.herokuapp.com``
- Ensure ``Socket Manager`` script in ``SocketManagerProd`` has ``Point Prefab`` variable referenced to "Assets/Scenes/MainScene/Point" game object or any game object of your choice
- Build and run the project
