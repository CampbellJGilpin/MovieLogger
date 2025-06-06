CREATE TABLE customers (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    phone_number VARCHAR(100) NOT NULL,
    date_of_birth TIMESTAMP NOT NULL,
    loyalty_points INT NOT NULL
);

CREATE TABLE service_categories (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT
);

CREATE TABLE services (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT,
    price NUMERIC(10, 2) NOT NULL,
    duration_minutes INT NOT NULL,
    category_id INT NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    is_promotional BOOLEAN DEFAULT FALSE,
    promotional_price NUMERIC(10, 2),
    CONSTRAINT fk_services_service_category_id FOREIGN KEY (category_id) REFERENCES service_categories(id)
);

CREATE TABLE appointments (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL,
    service_id INT NOT NULL,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL,
    status TEXT,
    notes TEXT,
    CONSTRAINT fk_appointments_customer_id FOREIGN KEY (customer_id) REFERENCES customers(id),
    CONSTRAINT fk_appointments_service_id FOREIGN KEY (service_id) REFERENCES services(id)
);

CREATE TABLE packages (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE service_packages (
    package_id INT NOT NULL,
    service_id INT NOT NULL,
    PRIMARY KEY (package_id, service_id),
    CONSTRAINT fk_service_packages_package_id FOREIGN KEY (package_id) REFERENCES packages(id),
    CONSTRAINT fk_service_packages_service_id FOREIGN KEY (service_id) REFERENCES services(id)
);
