# --- build stage -------------------------------------------------------------
FROM node:22-alpine AS build
WORKDIR /app

# VITE_API_URL is baked in at build time (Vite inlines import.meta.env).
# Override for a real deployment: --build-arg VITE_API_URL=https://api.example.com/api
ARG VITE_API_URL=http://localhost:8080/api
ENV VITE_API_URL=$VITE_API_URL

COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# --- serve stage -------------------------------------------------------------
FROM nginx:alpine AS serve
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
