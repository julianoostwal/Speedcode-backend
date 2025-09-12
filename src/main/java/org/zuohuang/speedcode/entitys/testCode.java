package org.zuohuang.speedcode.entitys;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import lombok.Data;

import java.util.UUID;

@Data
@Entity
public class testCode {

    @Id
    private UUID id;

}
