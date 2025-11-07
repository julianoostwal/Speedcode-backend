BEGIN;

DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_room_users_profiles'
    ) THEN
        ALTER TABLE room_users 
        ADD CONSTRAINT fk_room_users_profiles 
        FOREIGN KEY ("profileId") 
        REFERENCES profiles(id) 
        ON DELETE CASCADE;
        
        RAISE NOTICE 'Created: fk_room_users_profiles';
    END IF;
END $$;

DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_rooms_profiles'
    ) THEN
        ALTER TABLE rooms 
        ADD CONSTRAINT fk_rooms_profiles 
        FOREIGN KEY ("CreatedById") 
        REFERENCES profiles(id) 
        ON DELETE CASCADE;
        
        RAISE NOTICE 'Created: fk_rooms_profiles';
    END IF;
END $$;
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_room_exercises_exercises'
    ) THEN
        ALTER TABLE room_exercises 
        ADD CONSTRAINT fk_room_exercises_exercises 
        FOREIGN KEY ("exerciseId") 
        REFERENCES exercises(id) 
        ON DELETE CASCADE;
        
        RAISE NOTICE 'Created: fk_room_exercises_exercises';
    END IF;
END $$;

DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_completed_profile_exercises_exercises'
    ) THEN
        ALTER TABLE completed_profile_exercises 
        ADD CONSTRAINT fk_completed_profile_exercises_exercises 
        FOREIGN KEY ("exerciseId") 
        REFERENCES exercises(id) 
        ON DELETE CASCADE;
        
        RAISE NOTICE 'Created: fk_completed_profile_exercises_exercises';
    END IF;
END $$;

DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_created_profile_exercises_exercises'
    ) THEN
        ALTER TABLE created_profile_exercises 
        ADD CONSTRAINT fk_created_profile_exercises_exercises 
        FOREIGN KEY ("exerciseId") 
        REFERENCES exercises(id) 
        ON DELETE CASCADE;
        
        RAISE NOTICE 'Created: fk_created_profile_exercises_exercises';
    END IF;
END $$;

COMMIT;
SELECT 
    tc.table_name AS "Table", 
    tc.constraint_name AS "Constraint Name", 
    kcu.column_name AS "Column",
    ccu.table_name AS "References Table",
    ccu.column_name AS "References Column",
    rc.delete_rule AS "On Delete"
FROM 
    information_schema.table_constraints AS tc 
    JOIN information_schema.key_column_usage AS kcu
      ON tc.constraint_name = kcu.constraint_name
      AND tc.table_schema = kcu.table_schema
    JOIN information_schema.constraint_column_usage AS ccu
      ON ccu.constraint_name = tc.constraint_name
      AND ccu.table_schema = tc.table_schema
    JOIN information_schema.referential_constraints AS rc
      ON tc.constraint_name = rc.constraint_name
WHERE 
    tc.constraint_type = 'FOREIGN KEY'
    AND tc.constraint_name LIKE 'fk_%'
    AND (
        tc.constraint_name LIKE '%_profile%' 
        OR tc.constraint_name LIKE '%_exercise%'
        OR tc.constraint_name LIKE '%_room%'
    )
ORDER BY tc.table_name, tc.constraint_name;

