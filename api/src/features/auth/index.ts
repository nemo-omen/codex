import { Context, Hono, HonoRequest } from "hono";

const app = new Hono();

app.post('/login', async (c: Context) => {
    const result = await c.var.db.execute(`SELECT 1`);
    console.log(result);
    return c.json({ message: 'Login successful' });
});

app.post('/register', (c: Context) => {
    return c.json({ message: 'Registration successful' });
});

app.post('/logout', (c: Context) => {
    return c.json({ message: 'Logout successful' });
});

app.get('/user', (c: Context) => {
    return c.json({ user: { id: 1, name: 'John Doe' } });
});


export default app;