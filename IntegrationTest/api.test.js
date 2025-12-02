const axios = require('axios');
const https = require('https');

// === НАЛАШТУВАННЯ ===
const BASE_URL = 'https://localhost:7296/api'; 

// Ігноруємо помилки SSL сертифікатів (для локальної розробки)
const agent = new https.Agent({ rejectUnauthorized: false });
const client = axios.create({ 
    baseURL: BASE_URL,
    httpsAgent: agent,
    validateStatus: () => true 
});

describe('Integration Tests for Business Analytics API', () => {

    // --- ТЕСТ 1: Перевірка, чи сервер живий ---
    test('Health Check: GET /analytics/data returns 200', async () => {
        console.log(`Connecting to ${BASE_URL}/analytics/data ...`);
        try {
            const response = await client.get('/analytics/data');
            expect(response.status).toBe(200);
            expect(Array.isArray(response.data)).toBe(true);
            console.log(`[Success] Loaded ${response.data.length} records.`);
        } catch (error) {
            console.error("Connection failed. Is the server running?", error.message);
            throw error;
        }
    });

    // --- ТЕСТ 2: Повний цикл CRUD для Categories ---
    test('CRUD Cycle for Categories', async () => {
        const timestamp = new Date().getTime();
        const newCategory = { 
            name: `TestCategory_${timestamp}`, 
            description: "Integration Test Category" 
        };

        // 1. CREATE (Створення)
        console.log('1. Creating Category...');
        const createRes = await client.post('/v1/categories', newCategory);
        
        if (createRes.status !== 201) {
            console.error('Create failed. Status:', createRes.status, 'Data:', createRes.data);
        }
        expect(createRes.status).toBe(201);
        expect(createRes.data.name).toBe(newCategory.name);
        
        const createdId = createRes.data.id;
        expect(createdId).toBeDefined();

        // 2. READ (Читання по ID)
        console.log(`2. Reading Category ID: ${createdId}...`);
        const getRes = await client.get(`/v1/categories/${createdId}`);
        expect(getRes.status).toBe(200);
        expect(getRes.data.id).toBe(createdId);

        // 3. UPDATE (Оновлення)
        console.log(`3. Updating Category ID: ${createdId}...`);
        const updatedCategory = { 
            id: createdId,
            name: `Updated_Cat_${timestamp}`,
            description: "Updated Description"
        };
        const updateRes = await client.put(`/v1/categories/${createdId}`, updatedCategory);
        
        // Очікуємо 204 NoContent, або 200 OK в залежності від реалізації
        expect([200, 204]).toContain(updateRes.status); 

        // Перевірка оновлення
        const checkUpdate = await client.get(`/v1/categories/${createdId}`);
        expect(checkUpdate.data.name).toBe(updatedCategory.name);

        // 4. DELETE (Видалення)
        console.log(`4. Deleting Category ID: ${createdId}...`);
        const delRes = await client.delete(`/v1/categories/${createdId}`);
        expect([200, 204]).toContain(delRes.status);

        // Перевірка, що запису більше немає (має бути 404)
        const checkDelete = await client.get(`/v1/categories/${createdId}`);
        expect(checkDelete.status).toBe(404);
    });
});