# Stage 1: Build the Angular SPA

FROM node:22-alpine AS build
WORKDIR /app

# Copy package files first for caching
COPY package*.json ./

# Install npm dependencies
RUN npm ci

# Copy source code and compile production bundle
COPY . .
RUN npm run build -- --configuration=production

# Stage 2: Serve with Nginx
FROM nginx:alpine AS final

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy compiled Angular output from build stage
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Expose web server port
EXPOSE 80

# Start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]
