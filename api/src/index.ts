import auth from './features/auth';
import { factory } from './appFactory';

const app = factory.createApp()
  .basePath('/api')
  .route('/auth', auth);

const port = 3000;

export default {
  port,
  fetch: app.fetch,
};
