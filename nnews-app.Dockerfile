# Stage 1: Build
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files
COPY nnews-app/package*.json ./

# Install dependencies with legacy peer deps to resolve React version conflicts
RUN npm install --legacy-peer-deps

# Copy source code
COPY nnews-app/ ./

# Set environment variables for Vite build
ARG VITE_API_URL=http://localhost:5004
ARG VITE_NNEWS_API_URL=http://localhost:5007
ARG NODE_ENV=development

ENV VITE_API_URL=${VITE_API_URL}
ENV VITE_NNEWS_API_URL=${VITE_NNEWS_API_URL}
ENV NODE_ENV=${NODE_ENV}

# Build the application
RUN npm run build

# Stage 2: Production
FROM nginx:alpine AS production

# Copy custom nginx config
COPY nnews-app/nginx.conf /etc/nginx/conf.d/default.conf

# Copy built files from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Create health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost/health || exit 1

# Expose port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
