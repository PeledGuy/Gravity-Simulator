import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation

# Constants and initial conditions
G = 1.0

# We'll define an N bodies system:
masses = np.array([100000.0, 100000.0, 100000.0])
positions = np.array([
    [0.0, 60.0],        # Star 1 (Top)
    [-51.96, -30.0],    # Star 2 (Bottom Left)
    [51.96, -30.0]      # Star 3 (Bottom Right)
])
# Given a slight clockwise rotational velocity nudge
velocities = np.array([
    [25.0, 0.0],        # Star 1 moving right
    [-12.5, 21.65],     # Star 2 moving up-left
    [-12.5, -21.65]     # Star 3 moving down-left
])

print("Universe state initialized.")
print(f"Tracking {len(masses)} bodies in a 2D coordinate plane.")


def calculate_forces(masses, positions):
    """
    Calculates the net gravitational force acting on every body.
    Returns an (N, 2) matrix of force vectors [Fx, Fy].
    """
    
    N = len(masses)

    forces = np.zeros((N, 2))

    # Loop through every planet (i) and compare it to every other planet (j)
    for i in range(N):
        for j in range(N):
            if i == j:
                continue

            # Distance Vector: Pointing from planet i to planet j
            r_vec = positions[j] - positions[i]

            #2 The absolute distance in space
            r_mag = np.linalg.norm(r_vec)

            if r_mag == 0:
                continue

            # Newton's Law of Gravitation (Vector Form)
            force_mag = G * masses[i] * masses[j] / (r_mag ** 3)
            force_vec = force_mag * r_vec

            forces[i] += force_vec
    return forces

def update_states(masses, positions, velocities, dt):
    """
    Updates the physical state of the simulation by moving it forward 
    in time by a small step (dt).
    """

    forces = calculate_forces(masses, positions)

    # Calculate acceleration and new velocities and positions
    accelerations = forces / masses[:, np.newaxis]
    velocities += accelerations * dt
    positions += velocities * dt

    return positions, velocities

# Set up canvas
fig, ax = plt.subplots(figsize=(6, 6))
fig.canvas.manager.set_window_title('N-Body Gravity Engine')

# Set up backgrounds and camera limits
ax.set_facecolor('black')
ax.set_xlim(-150, 150)
ax.set_ylim(-150, 150)

# Initialize the Scatter Plot for the planets
colors = ['yellow', 'cyan', 'magenta']
sizes = [300, 300, 300]
scatter = ax.scatter(positions[:, 0], positions[:, 1], c=colors, s=sizes, zorder=2)

# Adding trails
N = len(masses)
# Create empty lists to store the past X and Y coordinates of every planet
history_x = [[] for _ in range(N)]
history_y = [[] for _ in range(N)]

# Create empty Line2D objects for Matplotlib to draw the trails
lines = []
for color in colors:
    line, = ax.plot([], [], color=color, alpha=0.5, linewidth=1.5, zorder=2, linestyle='--')
    lines.append(line)

# The animation loop
def animate(frame):
    global positions, velocities
    dt = 0.01

    for _ in range(10):
        positions, velocities = update_states(masses, positions, velocities, dt)

    for i in range(N):
        # Save the current position to the history list
        history_x[i].append(positions[i, 0])
        history_y[i].append(positions[i, 1])
        
        # If the trail gets longer than 300 frames, delete the oldest coordinate
        if len(history_x[i]) > 300:
            history_x[i].pop(0)
            history_y[i].pop(0)
            
        # Redraw line
        lines[i].set_data(history_x[i], history_y[i])

    scatter.set_offsets(positions)
    return [scatter] + lines

ani = animation.FuncAnimation(fig, animate, frames=2000, interval=16, blit=True)

plt.title("2-Body Orbital Simulation")
plt.show()