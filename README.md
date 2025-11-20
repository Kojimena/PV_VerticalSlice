# SpaceJunk — Vertical Slice

[Video Juego Completo](https://youtu.be/Zlh8_OhRE8M?si=KB2uNOtILn9vbDEm)

Prototipo jugable que demuestra el game loop principal: sobrevivir en el planeta, reunir piezas de la nave y escapar.

## Conceptos implementados
- Sistema de físicas.
- Raycasting.
- UI
- Audio
- Sistema de navegación
- Animación
- Data driven Design
- Persistencia de datos
- Manejo de escenas
- Menús de inicio,carga, pausa, victoria y derrota.

## Cómo correrlo
- Abrir el proyecto.
- Dar Play desde la escena `MainMenu` para ver el flujo completo con pantallas de carga, victoria y derrota.

## Game loop del slice
- El juego se debe iniciar en `MainMenu`, en donde "Iniciar una nueva aventura" resetea el progreso y "continuar" carga el último historial guardado.
- En `Level1` se puede explorar, recolectar piezas de la nave y vidas.
- Se puede usar el rover para desplazarse más rápido.
- Se deben evitar enemigos que patrullan/atacan. 
- Perder todas las vidas lleva a `LostMenu`.
- Al reunir todas las piezas, se debe interactuar con la nave averiada para repararla y saltar a `WinMenu`, si aún faltan piezas muestra checklist de piezas pendientes.
- Se puede poner pausa con `Esc` (menú con resume/restart/quit), se puede ocultar y mostrar el inventario con `Tab`.

## Controles
- Movimiento a pie: `WASD` / flechas, girar con el mouse, `Space` para saltar.
- Rover: `E` para subir/bajar; `W/S` aceleran/retrocede, `A/D` giran.
- Interacción: caminar sobre pickups; `Tab` abre/cierra inventario; `Esc` pausa.

## Escenas
- `MainMenu`: flujo de inicio/continuar/salir.
- `LoadingScene`: pantalla intermedia asíncrona con barra de progreso.
- `Level1`: nivel jugable con enemigos, pickups, rover, nave y menú de pausa.
- `LostMenu` y `WinMenu`: pantallas de fin de partida.
